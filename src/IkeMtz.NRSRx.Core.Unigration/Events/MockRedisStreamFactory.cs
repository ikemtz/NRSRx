using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  public static class MockRedisStreamFactory<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
   where TEntity : class, IIdentifiable<TIdentityType>
   where TEvent : EventType, new()
  {
    public static Mock<IPublisher<TEntity, TEvent, TIdentityType>> CreatePublisher()
    {
      return new Mock<IPublisher<TEntity, TEvent, TIdentityType>>();
    }
    public static (Mock<IConnectionMultiplexer> Connection, Mock<IDatabase> Database) CreateConnection()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      var database = new Mock<IDatabase>();
      _ = connection.Setup(t => t.GetDatabase(It.IsAny<int>(), It.Is<object>(t => t == null))).Returns(database.Object);
      return (connection, database);
    }

    public static (Mock<RedisStreamSubscriber<TEntity, TEvent, TIdentityType>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity>? collection = null)
    {
      var (connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent, TIdentityType>>(new object[] { connection.Object });
      SetupMockSubscriberCollection(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    public static void SetupMockSubscriberCollection<TSubscriberType>(Mock<TSubscriberType> mockSubscriber, IEnumerable<TEntity>? collection = null)
      where TSubscriberType : RedisStreamSubscriber<TEntity, TEvent, TIdentityType>
    {
      if (collection != null)
      {
        _ = mockSubscriber
          .Setup(t => t.GetMessagesAsync(It.IsAny<int>()))
          .ReturnsAsync(ExpandWithRedisValues(collection));
        _ = mockSubscriber
          .Setup(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()))
          .ReturnsAsync(1);
        _ = mockSubscriber
          .Setup(t => t.GetStreamInfoAsync())
          .ReturnsAsync(new MessageQueueInfo
          {
            DeadLetterCount = 0,
            MessageCount = collection.Count(),
            SubscriberCount = 1,

          });
      }
    }

    public static IEnumerable<(RedisValue Id, TEntity Entity)> ExpandWithRedisValues(IEnumerable<TEntity>? collection = null)
    {
      return collection?.Select(t => (new RedisValue(t.Id.ToString()), t));
    }

  }
}
