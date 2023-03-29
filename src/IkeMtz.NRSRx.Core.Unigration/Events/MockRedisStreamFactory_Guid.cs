using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Microsoft.EntityFrameworkCore.Metadata;
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
      return MockRedisStreamFactory.CreateMockConnection();
    }

    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity>? collection = null)
    {
      var (connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>(new object[] { connection.Object, new RedisSubscriberOptions() });
      SetupMockSubscriberCollection(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(Func<IEnumerable<TEntity>> getCollection)
    {
      var (connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>(new object[] { connection.Object, new RedisSubscriberOptions() });
      IEnumerable<TEntity> collection = Array.Empty<TEntity>();
      _ = mockSubscriber
        .Setup(t => t.GetMessagesAsync(It.IsAny<int>()))
        .ReturnsAsync(() => ExpandWithRedisValues(collection = getCollection()));
      MockRedisStreamFactory<TEntity, TEvent, Guid>.SetupSupportMockMethods(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    public static void SetupMockSubscriberCollection(Mock<RedisStreamSubscriber<TEntity, TEvent>> mockSubscriber, IEnumerable<TEntity>? collection = null)
    {
      MockRedisStreamFactory<TEntity, TEvent, Guid>.SetupMockSubscriberCollection(mockSubscriber, collection);
    }

    public static IEnumerable<(RedisValue Id, TEntity Entity)> ExpandWithRedisValues(IEnumerable<TEntity>? collection = null)
    {
      return collection?.Select(t => (new RedisValue(t.Id.ToString("N")), t));
    }
  }
}
