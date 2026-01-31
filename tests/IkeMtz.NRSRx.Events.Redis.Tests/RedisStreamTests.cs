using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;
using static IkeMtz.NRSRx.Core.Unigration.TestDataFactory;
using RedisStreamConsumerMetadata = IkeMtz.NRSRx.Events.Subscribers.Redis.RedisStreamConsumerMetadata;

namespace IkeMtz.NRSRx.Events.Publishers.Redis.Tests
{
  [TestClass]
  [DoNotParallelize]
  public class RedisStreamTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
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
      foreach (var (Id, Entity) in subscribedMessages)
      {
        var count = await subscriber.AcknowledgeMessageAsync(Id);
        Assert.AreEqual(1, count);
      }
      var deletedCount = await subscriber.DeleteIdleConsumersAsync();

    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
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
    [TestCategory(TestCategories.Integration)]
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
      Assert.AreNotEqual(0, currentStreamInfoA.MsgCount);
      Assert.AreNotEqual(0, currentStreamInfoA.AcknowledgedMsgCount);


      var currentStreamInfoB = await subscriberB.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfoB.MsgCount);
      Assert.AreNotEqual(0, currentStreamInfoB.AcknowledgedMsgCount);

      _ = await subscriberA.DeleteIdleConsumersAsync();
      _ = await subscriberA.Database.StreamDeleteConsumerGroupAsync(subscriberA.StreamKey, subscriberA.ConsumerGroupName);

      _ = await subscriberB.DeleteIdleConsumersAsync();
      _ = await subscriberB.Database.StreamDeleteConsumerGroupAsync(subscriberA.StreamKey, subscriberA.ConsumerGroupName);
    }



    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("RedisIntegration")]
    public async Task ValidateGetPendingSingleConsumerGroupsAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(connectionMultiplexer);
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(connectionMultiplexer);
      _ = subscriber.Init();
      var sampleMessage = new SampleMessage();
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      await publisher.PublishAsync(sampleMessage);

      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      var subscribedMessages = await subscriber.GetMessagesAsync();
      Assert.AreEqual(original.Length + 1, result.Length);
      Thread.Sleep(10000);
      subscribedMessages = await subscriber.GetPendingMessagesAsync();
      Assert.AreNotEqual(0, subscribedMessages.Count());
      subscribedMessages.ToList().ForEach(async x => await subscriber.AcknowledgeMessageAsync(x.Id));
    }


    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("RedisIntegration")]
    public async Task PushSchoolEventsAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<School, CreateEvent>(connectionMultiplexer);
      for (int i = 0; i < 20; i++)
      {
        var schoolMsg = new School
        {
          FullName = StringGenerator(50, true, CharacterSets.UpperCase),
          Name = StringGenerator(10, false, CharacterSets.UpperCase),
          Id = Guid.NewGuid()
        };
        await publisher.PublishAsync(schoolMsg);
      }

      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.IsLessThanOrEqualTo(result.Length, 10);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
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
    [TestCategory(TestCategories.Integration)]
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
      _ = await publisher.Database.KeyDeleteAsync(subscriber.ConsumerGroupAckCounterKey);
    }


    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateRedisMoqPublishAsync()
    {
      var (Connection, Database) = MockRedisStreamFactory.CreateMockConnection();
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent>(Connection.Object);
      var msg = new SampleMessage();
      await publisher.PublishAsync(msg);
      Database
        .Verify(t => t.StreamAddAsync(publisher.StreamKey, It.Is<RedisValue>(x => x.Equals(nameof(SampleMessage))), It.IsAny<RedisValue>(), null, null, false, null, StreamTrimMode.KeepReferences, CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateRedisMoqSubscriberGetMessagesAsync()
    {
      var (Connection, Database) = MockRedisStreamFactory.CreateMockConnection();
      //Database.StreamReadGroupAsync(StreamKey, ConsumerGroupName, ConsumerName.GetValueOrDefault(), count: messageCount ?? Options.MessagesPerBatchCount)
      _ = Database
        .Setup(x => x.StreamReadGroupAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), 5, false, CommandFlags.None))
        .Returns(Task.FromResult(new StreamEntry[] {
          new("a", [new("letter", "a")]),
          new("b", [new("letter", "b")]),
        }));
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(Connection.Object);
      _ = subscriber.Init();
      var result = await subscriber.GetMessagesAsync();
      Database
        .Verify(t => t.StreamReadGroupAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.GetValueOrDefault(), null, 5, false, null, It.IsAny<CommandFlags>()), Times.Once);
    }
    class RedisStreamSubscriberMock(IConnectionMultiplexer connection) : RedisStreamSubscriber<SampleMessage, CreateEvent>(connection)
    {
      public override Task<IEnumerable<RedisStreamConsumerMetadata>> GetIdleConsumersWithPendingMsgsAsync() =>
        Task.FromResult<IEnumerable<RedisStreamConsumerMetadata>>([new RedisStreamConsumerMetadata { Name = "Unit Test" }]);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateRedisMoqSubscriberGetPendingMessagesAsync()
    {
      var (Connection, Database) = MockRedisStreamFactory.CreateMockConnection();

      _ = Database
        .Setup(x => x.StreamPendingMessagesAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<RedisValue>(), null, null, null, CommandFlags.None))
        .Returns(Task.FromResult(new[] { new StreamPendingMessageInfo() }));

      var subscriber = new RedisStreamSubscriberMock(Connection.Object);
      _ = subscriber.Init();
      _ = await subscriber.GetPendingMessagesAsync();
      Database
         .Verify(t => t.StreamPendingMessagesAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, It.IsAny<int>(), "Unit Test", null, null, null, CommandFlags.None),
         Times.Once);
      Database
         .Verify(t => t.StreamClaimAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, subscriber.ConsumerName.GetValueOrDefault(), 10000, It.IsAny<RedisValue[]>(), CommandFlags.None), Times.Exactly(2));

    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateGetConsumersWithPendingMessagesTest()
    {
      var (Connection, Database) = MockRedisStreamFactory.CreateMockConnection();
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(Connection.Object);
      var result = await subscriber.GetIdleConsumersWithPendingMsgsAsync();
      Assert.AreEqual(0, result.Count());
      Database
        .Verify(t => t.StreamConsumerInfoAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateSubscriptionState()
    {
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateSubscriber();
      await Assert.ThrowsExactlyAsync<InvalidProgramException>(async () => await Subscriber.Object.Subscribe());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateSubscriptionIdentityTypeState()
    {
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent, Guid>.CreateSubscriber();
      await Assert.ThrowsExactlyAsync<InvalidProgramException>(async () => await Subscriber.Object.Subscribe());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateMessageAcknowledgement()
    {
      var (Connection, Database) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateConnection();
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(Connection.Object);
      _ = await subscriber.AcknowledgeMessageAsync("xyz");
      Database
       .Verify(t => t.StreamAcknowledgeAsync(subscriber.StreamKey, subscriber.ConsumerGroupName, "xyz", CommandFlags.None), Times.Once);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateMockMessageRecieved()
    {
      var message = new SampleMessage();
      var (Subscriber, _) = MockRedisStreamFactory<SampleMessage, CreateEvent>.CreateSubscriber([message]);
      var messages = await Subscriber.Object.GetMessagesAsync(1);
      Assert.AreEqual(message.Name, messages.First().Entity.Name);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateSubscriberMessageRecieved()
    {
      var (Connection, _) = MockRedisStreamFactory.CreateMockConnection();
      var subscriber = new RedisStreamSubscriber<SampleMessage, CreateEvent>(Connection.Object);
      var messages = await subscriber.GetMessagesAsync(1);
      Assert.IsFalse(messages.Any());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ValidateMessageCoderJsonDecode()
    {
      var json = "eyJpZCI6IjA5MDhlOGM0LWJhOGEtNDQyNy1hNjJmLTkxOTEyZWU3NmUyNiIsIm5hbWUiOiI1MWM5ZTRkYy00ZDc1LTQxNGQtOWFmOS00NDJjNGViNTM2YzIifQ==";
      var buffer = Convert.FromBase64String(json);
      var message = MessageCoder.JsonDecode<SampleMessage>(buffer);
      Assert.AreEqual("51c9e4dc-4d75-414d-9af9-442c4eb536c2", message.Name);
    }
  }
}
