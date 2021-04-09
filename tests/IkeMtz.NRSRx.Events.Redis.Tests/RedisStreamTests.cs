using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis.Tests
{
  [TestClass]
  public class RedisStreamTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task ValidateRedisPublishAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, StreamPosition.NewMessages);
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessages = await subscriber.GetMessagesAsync();
      var (Id, Entity) = subscribedMessages.First();
      Assert.AreEqual(sampleMessage.Name, Entity.Name);
      var count = await subscriber.AcknowledgeMessageAsync(Id);
      Assert.AreEqual(1, count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqPublishAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(moqConnection.Object);
      var msg = new SampleMessage();
      await publisher.PublishAsync(msg);
      moqDatabase
        .Verify(t => t.StreamAddAsync(publisher.StreamKey, It.Is<RedisValue>(x => x.StartsWith(msg.Id.ToString())), It.IsAny<RedisValue>(), null, null, false, CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqSubscribeAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);

      await subscriber.GetMessagesAsync();
      moqDatabase
        .Verify(t => t.StreamReadGroupAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.InstanceId.ToString("N"), null, 1, false, CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(InvalidProgramException))]
    public async Task ValidateSubscriptionState()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);
      await subscriber.Subscribe();
    }

  }
}
