using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  public static class MockRedisStreamFactory<TEntity, TEvent>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public static Mock<RedisStreamPublisher<TEntity, TEvent>> CreatePublisher()
    {
      var (Connection, _) = CreateConnection();
      return new Mock<RedisStreamPublisher<TEntity, TEvent>>(new[] { Connection.Object });
    }
    public static (Mock<IConnectionMultiplexer> Connection, Mock<IDatabase> Database) CreateConnection()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      var database = new Mock<IDatabase>();
      connection.Setup(t => t.GetDatabase(It.IsAny<int>(), It.Is<object>(t => t == null))).Returns(database.Object);
      return (connection, database);
    }

    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity> collection = null)
    {
      var (Connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>(new object[] { Connection.Object, StreamPosition.NewMessages.ToString() });
      if (collection != null)
      {
        mockSubscriber
          .Setup(t => t.GetMessagesAsync(It.Is<int>(t => t == collection.Count())))
          .Returns(Task.FromResult(collection.Select(t => (new RedisValue(t.Id.ToString()), t))));
      }
      return (mockSubscriber, database);
    }
  }
}
