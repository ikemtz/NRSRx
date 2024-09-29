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
  /// <summary>
  /// Factory for creating mock Redis stream publishers and subscribers for testing purposes.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public static class MockRedisStreamFactory<TEntity, TEvent>
      where TEntity : class, IIdentifiable<Guid>
      where TEvent : EventType, new()
  {
    /// <summary>
    /// Creates a mock publisher.
    /// </summary>
    /// <returns>A mock publisher.</returns>
    public static Mock<IPublisher<TEntity, TEvent>> CreatePublisher()
    {
      return new Mock<IPublisher<TEntity, TEvent>>();
    }

    /// <summary>
    /// Creates a mock Redis connection and database.
    /// </summary>
    /// <returns>A tuple containing the mock connection and database.</returns>
    public static (Mock<IConnectionMultiplexer> Connection, Mock<IDatabase> Database) CreateConnection()
    {
      return MockRedisStreamFactory.CreateMockConnection();
    }

    /// <summary>
    /// Creates a mock subscriber and database.
    /// </summary>
    /// <param name="collection">An optional collection of entities to initialize the subscriber with.</param>
    /// <returns>A tuple containing the mock subscriber and database.</returns>
    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity>? collection = null)
    {
      var (connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>([connection.Object, new RedisSubscriberOptions()]);
      SetupMockSubscriberCollection(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    /// <summary>
    /// Creates a mock subscriber and database with a function to get the collection.
    /// </summary>
    /// <param name="getCollection">A function to get the collection of entities.</param>
    /// <returns>A tuple containing the mock subscriber and database.</returns>
    public static (Mock<RedisStreamSubscriber<TEntity, TEvent>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(Func<IEnumerable<TEntity>> getCollection)
    {
      var (connection, database) = CreateConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent>>([connection.Object, new RedisSubscriberOptions()]);
      IEnumerable<TEntity> collection = [];
      _ = mockSubscriber
        .Setup(t => t.GetMessagesAsync(It.IsAny<int>()))
        .ReturnsAsync(() => ExpandWithRedisValues(collection = getCollection()));
      MockRedisStreamFactory<TEntity, TEvent, Guid>.SetupSupportMockMethods(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    /// <summary>
    /// Sets up the mock subscriber with a collection of entities.
    /// </summary>
    /// <param name="mockSubscriber">The mock subscriber.</param>
    /// <param name="collection">An optional collection of entities to initialize the subscriber with.</param>
    public static void SetupMockSubscriberCollection(Mock<RedisStreamSubscriber<TEntity, TEvent>> mockSubscriber, IEnumerable<TEntity>? collection = null)
    {
      MockRedisStreamFactory<TEntity, TEvent, Guid>.SetupMockSubscriberCollection(mockSubscriber, collection);
    }

    /// <summary>
    /// Expands a collection of entities with Redis values.
    /// </summary>
    /// <param name="collection">The collection of entities.</param>
    /// <returns>A collection of tuples containing Redis values and entities.</returns>
    public static IEnumerable<(RedisValue Id, TEntity Entity)> ExpandWithRedisValues(IEnumerable<TEntity>? collection = null)
    {
      return collection?.Select(t => (new RedisValue(t.Id.ToString("N")), t)) ?? [];
    }
  }
}
