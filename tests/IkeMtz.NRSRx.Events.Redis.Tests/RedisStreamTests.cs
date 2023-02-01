using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;
using Consumer = IkeMtz.NRSRx.Events.Subscribers.Redis.Consumer;

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
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions { IdleTimeSpanInMilliseconds = 1 });
      _ = subscriber.Init();
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessages = await subscriber.GetMessagesAsync(10);
      var ids = subscribedMessages.Select(t => t.Entity.Id).ToArray();
      Assert.IsTrue(ids.Any(a => a.Equals(sampleMessage.Id)));
      foreach (var msg in subscribedMessages)
      {
        var count = await subscriber.AcknowledgeMessageAsync(msg.Id);
        Assert.AreEqual(1, count);
      }
      var deletedCount = await subscriber.DeleteIdleConsumersAsync();
      Assert.AreNotEqual(0, deletedCount);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateMultipleConsumersWithPendingAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriberA = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions { IdleTimeSpanInMilliseconds = 1 });
      var subscriberB = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions { IdleTimeSpanInMilliseconds = 1 });

      _ = subscriberA.Init();
      _ = subscriberB.Init();
      var sampleMessage = new SampleMessage();
      await publisher.PublishAsync(sampleMessage);

      var subscribedMessages = await subscriberA.GetMessagesAsync(10);
      Assert.AreNotEqual(0, subscribedMessages.Count());
      Thread.Sleep(5);
      subscribedMessages = await subscriberA.GetMessagesAsync(10);
      Assert.AreEqual(0, subscribedMessages.Count());
      Thread.Sleep(30000);
      subscribedMessages = await subscriberB.GetPendingMessagesAsync(10);
      Assert.AreNotEqual(0, subscribedMessages.Count());
      var count = 0L;
      foreach (var (Id, Entity) in subscribedMessages)
      {
        count += await subscriberB.AcknowledgeMessageAsync(Id);
      }
      Assert.AreNotEqual(0, count);
      subscribedMessages = await subscriberA.GetPendingMessagesAsync();
      Assert.AreEqual(0, subscribedMessages.Count());
      subscribedMessages = await subscriberB.GetPendingMessagesAsync();
      Assert.AreEqual(0, subscribedMessages.Count());
      var deletedCount = await subscriberA.DeleteIdleConsumersAsync();
      Assert.AreNotEqual(0, deletedCount);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateMultipleConsumerGroupsAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriberA = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions
      {
        ConsumerGroupName = Guid.NewGuid().ToString(),
        IdleTimeSpanInMilliseconds = 1,
      });
      var subscriberB = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions
      {
        ConsumerGroupName = Guid.NewGuid().ToString(),
        IdleTimeSpanInMilliseconds = 1,
      });

      Assert.IsTrue(subscriberA.Init());
      Assert.IsTrue(subscriberB.Init());
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      await publisher.PublishAsync(sampleMessage);
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
      var subscribedMessagesA = await subscriberA.GetMessagesAsync();
      Assert.AreEqual(1, subscribedMessagesA.Count());
      var subscribedMessagesB = await subscriberB.GetMessagesAsync();
      Assert.AreEqual(1, subscribedMessagesB.Count());
      var count = 0L;
      foreach (var (Id, Entity) in subscribedMessagesA)
      {
        count += await subscriberB.AcknowledgeMessageAsync(Id);
      }
      Assert.AreNotEqual(0, count);

      foreach (var (Id, Entity) in subscribedMessagesB)
      {
        count += await subscriberA.AcknowledgeMessageAsync(Id);
      }
      Assert.AreNotEqual(0, count);

      var currentStreamInfoA = await subscriberA.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfoA.MessageCount);
      Assert.AreNotEqual(0, currentStreamInfoA.AckMessageCount);


      var currentStreamInfoB = await subscriberB.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfoB.MessageCount);
      Assert.AreNotEqual(0, currentStreamInfoB.AckMessageCount);

      _ = await subscriberA.DeleteIdleConsumersAsync();
      _ = await subscriberA.Database.StreamDeleteConsumerGroupAsync(subscriberA.StreamKey, subscriberA.ConsumerGroupName);

      _ = await subscriberB.DeleteIdleConsumersAsync();
      _ = await subscriberB.Database.StreamDeleteConsumerGroupAsync(subscriberA.StreamKey, subscriberA.ConsumerGroupName);
    }


    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateConsumerGroupNewMessagePositionAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");

      var publisher = new RedisStreamPublisher<Course, CreateEvent>(connectionMultiplexer);
      var rand = new Random();
      for (int i = 0; i < 10; i++)
      {
        var course = new Course
        {
          Num = Guid.NewGuid().ToString(),
          AvgScore = rand.NextDouble(),
          Id = Guid.NewGuid(),
        };
        await publisher.PublishAsync(course);
      }

      var subscriber = new RedisStreamSubscriber<Course, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions
      {
        IdleTimeSpanInMilliseconds = 1,
        StartPosition = StreamPosition.NewMessages,
      });

      Assert.IsTrue(subscriber.Init());

      var subscribedMessages = await subscriber.GetMessagesAsync();
      Assert.AreEqual(0, subscribedMessages.Count());
      _ = await publisher.Database.KeyDeleteAsync(publisher.StreamKey);
    }



    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task ValidateConsumerGroupBeginningPositionAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");

      var publisher = new RedisStreamPublisher<Course, CreateEvent>(connectionMultiplexer);
      var rand = new Random();
      for (int i = 0; i < 10; i++)
      {
        var course = new Course
        {
          Num = Guid.NewGuid().ToString(),
          AvgScore = rand.NextDouble(),
          Id = Guid.NewGuid(),
        };
        await publisher.PublishAsync(course);
      }

      var subscriber = new RedisStreamSubscriber<Course, CreateEvent>(connectionMultiplexer, new RedisSubscriberOptions
      {
        IdleTimeSpanInMilliseconds = 1,
        StartPosition = StreamPosition.Beginning,
      });
      _ = subscriber.Init();

      var subscribedMessages = await subscriber.GetMessagesAsync(20);
      Assert.AreEqual(10, subscribedMessages.Count());
      _ = await publisher.Database.KeyDeleteAsync(subscriber.StreamKey);
      _ = await publisher.Database.KeyDeleteAsync(subscriber.ConsumerGroupCounterKey);
    }


    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqPublishAsync()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      _ = moqConnection.Setup(t => t.GetDatabase(1, null)).Returns(moqDatabase.Object);
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(moqConnection.Object);
      var msg = new SampleMessage();
      await publisher.PublishAsync(msg);
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
      _ = moqConnection.Setup(t => t.GetDatabase(1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);
      _ = subscriber.Init();
      _ = await subscriber.GetMessagesAsync();
      moqDatabase
        .Verify(t => t.StreamReadGroupAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.GetValueOrDefault(), null, 1, false, CommandFlags.None), Times.Once);
    }
    class RedisStreamSubscriberMock : RedisStreamSubscriber<SampleMessage, CreateEvent>
    {
      public RedisStreamSubscriberMock(IConnectionMultiplexer connection) : base(connection)
      {
      }
      public override Task<IEnumerable<Consumer>> GetIdleConsumersWithPendingMsgsAsync() =>
        Task.FromResult<IEnumerable<Consumer>>(new Consumer[] { new Consumer { Name = "Unit Test" } });
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
      _ = moqConnection.Setup(t => t.GetDatabase(1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriberMock(moqConnection.Object);
      _ = subscriber.Init();
      _ = await subscriber.GetPendingMessagesAsync();
      moqDatabase
         .Verify(t => t.StreamPendingMessagesAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, 1, "Unit Test", null, null, CommandFlags.None), Times.Once);
      moqDatabase
        .Verify(t => t.StreamClaimAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.GetValueOrDefault(), 10000, It.IsAny<RedisValue[]>(), CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateGetConsumersWithPendingMessagesTest()
    {
      var moqConnection = new Mock<IConnectionMultiplexer>();
      var moqDatabase = new Mock<IDatabase>();
      _ = moqConnection.Setup(t => t.GetDatabase(1, null)).Returns(moqDatabase.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(moqConnection.Object);
      var result = await subscriber.GetIdleConsumersWithPendingMsgsAsync();
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
    [ExpectedException(typeof(InvalidProgramException))]
    public async Task ValidateSubscriptionIdentityTypeState()
    {
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent, Guid>.CreateSubscriber();
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
    public async Task ValidateSubscriberMessageRecieved()
    {
      var mockConnection = new Mock<IConnectionMultiplexer>();
      var database = new Mock<IDatabase>();
      _ = mockConnection.Setup(t => t.GetDatabase(1, null)).Returns(database.Object);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(mockConnection.Object);
      var messages = await subscriber.GetMessagesAsync(1);
      Assert.IsFalse(messages.Any());
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ValidateMessageCoderJsonDecode()
    {
      var json = "eyJpZCI6IjA5MDhlOGM0LWJhOGEtNDQyNy1hNjJmLTkxOTEyZWU3NmUyNiIsIm5hbWUiOiI1MWM5ZTRkYy00ZDc1LTQxNGQtOWFmOS00NDJjNGViNTM2YzIifQ==";
      var buffer = Convert.FromBase64String(json);
      var message = MessageCoder.JsonDecode<SampleMessage>(buffer);
      Assert.AreEqual("51c9e4dc4d75414d9af9442c4eb536c2", message.Name);
    }
  }
}
