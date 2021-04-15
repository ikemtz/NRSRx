using System;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
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
      moqDatabase
        .Setup(x => x.StreamReadGroupAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), 1, false, CommandFlags.None))
        .Returns(Task.FromResult(Array.Empty<StreamEntry>()));
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
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateSubscriber();
      await Subscriber.Object.Subscribe();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateMessageAcknowledgement()
    {
      var (Connection, Database) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateConnection();
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(Connection.Object);
      _ = await subscriber.AcknowledgeMessageAsync("xyz");
      Database
       .Verify(t => t.StreamAcknowledgeAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, "xyz", CommandFlags.None), Times.Once);
    }


    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateMockMessageRecieved()
    {
      var message = new SampleMessage();
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateSubscriber(new[] { message });
      var messages = await Subscriber.Object.GetMessagesAsync(1);
      Assert.AreEqual(message.Name, messages.First().Entity.Name);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ValidateMessageCoderJsonDecode()
    {
      var json = "eyJpZCI6IjA5MDhlOGM0LWJhOGEtNDQyNy1hNjJmLTkxOTEyZWU3NmUyNiIsIm5hbWUiOiI1MWM5ZTRkYy00ZDc1LTQxNGQtOWFmOS00NDJjNGViNTM2YzIifQ==";
      var buffer = Convert.FromBase64String(json);
      var message = MessageCoder.JsonDecode<SampleMessage>(buffer);
      Assert.AreEqual("51c9e4dc-4d75-414d-9af9-442c4eb536c2", message.Name);
    }
  }
}
