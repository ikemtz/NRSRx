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
  public static class MockRedisStreamFactory<TEntity, TEvent>
   where TEntity : class, IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public static Mock<IPublisher<TEntity, TEvent>> CreatePublisher()
    {
      return new Mock<IPublisher<TEntity, TEvent>>();
    }
    public static (Mock<IConnectionMultiplexer> Connection, Mock<IDatabase> Database) CreateConnection()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      var database = new Mock<IDatabase>();
      _ = connection.Setup(t => t.GetDatabase(It.IsAny<int>(), It.Is<object>(t => t == null))).Returns(database.Object);
      return (connection, database);
    }

    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity>? collection = null)
    {
      var (Connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>(new object[] { Connection.Object });
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
      return (mockSubscriber, database);
    }

    public static IEnumerable<(RedisValue Id, TEntity Entity)> ExpandWithRedisValues(IEnumerable<TEntity>? collection = null)
    {
      return collection?
        .Select(t => (new RedisValue(t.Id.ToString("N")), t));
    }
  }
}
