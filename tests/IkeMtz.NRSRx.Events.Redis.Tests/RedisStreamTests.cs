using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    [TestCategory("RedisIntegration")]
    public async Task ValidateRedisPublishAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer);
      _ = subscriber.Init(StreamPosition.NewMessages);
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      _ = await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessages = await subscriber.GetMessagesAsync();
      var (Id, Entity) = subscribedMessages.First();
      Assert.AreEqual(sampleMessage.Name, Entity.Name);
      var count = await subscriber.AcknowledgeMessageAsync(Id);
      Assert.AreEqual(1, count);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateRedisPendingAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriberA = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer)
      {
        ConsumerGroupName = Guid.NewGuid().ToString()
      };
      var subscriberB = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer)
      {
        ConsumerGroupName = subscriberA.ConsumerGroupName
      };

      Assert.IsTrue(subscriberA.Init(StreamPosition.NewMessages));
      Assert.IsFalse(subscriberB.Init(StreamPosition.NewMessages));
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      _ = await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessages = await subscriberA.GetMessagesAsync();
      Assert.AreEqual(1, subscribedMessages.Count());
      subscribedMessages = await subscriberA.GetMessagesAsync();
      Assert.AreEqual(0, subscribedMessages.Count());
      Thread.Sleep(15000);
      subscribedMessages = await subscriberB.GetPendingMessagesAsync();
      Assert.AreNotEqual(0, subscribedMessages.Count());
      var count = 0L;
      foreach (var (Id, Entity) in subscribedMessages)
      {
        count += await subscriberB.AcknowledgeMessageAsync(Id);
      }
      Assert.AreNotEqual(0, count);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateMultipleSubscribersSingleChannelAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriberA = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer)
      {
        ConsumerGroupName = Guid.NewGuid().ToString()
      };
      var subscriberB = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer)
      {
        ConsumerGroupName = Guid.NewGuid().ToString()
      };

      Assert.IsTrue(subscriberA.Init(StreamPosition.NewMessages));
      Assert.IsTrue(subscriberB.Init(StreamPosition.NewMessages));
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      _ = await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessages = await subscriberA.GetMessagesAsync();
      Assert.AreEqual(1, subscribedMessages.Count());
      subscribedMessages = await subscriberB.GetMessagesAsync();
      Assert.AreEqual(1, subscribedMessages.Count());
      var count = 0L;
      foreach (var (Id, Entity) in subscribedMessages)
      {
        count += await subscriberB.AcknowledgeMessageAsync(Id);
      }
      Assert.AreNotEqual(0, count);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqPublishAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      _ = moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(moqConnection.Object);
      var msg = new SampleMessage();
      _ = await publisher.PublishAsync(msg);
      moqDatabase
        .Verify(t => t.StreamAddAsync(publisher.StreamKey, It.Is<RedisValue>(x => x.StartsWith(msg.Id.ToString())), It.IsAny<RedisValue>(), null, null, false, CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqSubscriberGetMessagesAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      _ = moqDatabase
        .Setup(x => x.StreamReadGroupAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), 1, false, CommandFlags.None))
        .Returns(Task.FromResult(Array.Empty<StreamEntry>()));
      _ = moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);
      _ = subscriber.Init();
      _ = await subscriber.GetMessagesAsync();
      moqDatabase
        .Verify(t => t.StreamReadGroupAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.Value, null, 1, false, CommandFlags.None), Times.Once);
    }
    class RedisStreamSubscriberMock : RedisStreamSubscriber<SampleMessage, CreateEvent>
    {
      public RedisStreamSubscriberMock(IConnectionMultiplexer connection) : base(connection)
      {
      }
      public override Task<IEnumerable<(string ConsumerName, int PendingMessageCount)>> GetConsumersWithPendingMessagesAsync() =>
        Task.FromResult<IEnumerable<(string ConsumerName, int PendingMessageCount)>>(new[] { (ConsumerName: "Unit Test", PendingMessageCount: 100) });
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqSubscriberGetPendingMessagesAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();

      _ = moqDatabase
        .Setup(x => x.StreamPendingMessagesAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), 1, It.IsAny<RedisValue>(), null, null, CommandFlags.None))
        .Returns(Task.FromResult(new[] { new StreamPendingMessageInfo() }));
      _ = moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriberMock(moqConnection.Object);
      _ = subscriber.Init();
      _ = await subscriber.GetPendingMessagesAsync();
      moqDatabase
         .Verify(t => t.StreamPendingMessagesAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, 1, "Unit Test", null, null, CommandFlags.None), Times.Once);
      moqDatabase
        .Verify(t => t.StreamClaimAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.Value, 10000, It.IsAny<RedisValue[]>(), CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateGetConsumersWithPendingMessagesTest()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      _ = moqConnection.Setup(t => t.GetDatabase(-1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);
      var result = await subscriber.GetConsumersWithPendingMessagesAsync();
      Assert.IsNull(result);
      moqDatabase
        .Verify(t => t.StreamPendingAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, CommandFlags.None), Times.Once);
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
