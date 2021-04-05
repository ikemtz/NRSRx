using System;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  public static class MockRedisStreamPublisherFactory<TEntity, TEvent>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public static Mock<RedisStreamPublisher<TEntity, TEvent>> Create()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      connection.Setup(t => t.GetDatabase(It.Is<int>(t => t == -1), It.Is<object>(t => t == null))).Returns(new Mock<IDatabase>().Object);
      return new Mock<RedisStreamPublisher<TEntity, TEvent>>(new[] { connection.Object });
    }
    public static Mock<IConnectionMultiplexer> CreateConnection()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      connection.Setup(t => t.GetDatabase(It.Is<int>(t => t == -1), It.Is<object>(t => t == null))).Returns(new Mock<IDatabase>().Object);
      return connection;
    }
  }
}
