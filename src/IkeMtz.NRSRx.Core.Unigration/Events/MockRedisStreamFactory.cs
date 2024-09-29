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
  /// Factory for creating mock Redis stream connections and subscribers for testing purposes.
  /// </summary>
  public static class MockRedisStreamFactory
  {
    /// <summary>
    /// Creates a mock Redis connection and database.
    /// </summary>
    /// <returns>A tuple containing the mock connection and database.</returns>
    public static (Mock<IConnectionMultiplexer> Connection, Mock<IDatabase> Database) CreateMockConnection()
    {
      var connection = new Mock<IConnectionMultiplexer>();
      var database = new Mock<IDatabase>();
      _ = connection.Setup(t => t.GetDatabase(It.IsAny<int>(), It.Is<object>(t => t == null))).Returns(database.Object);
      return (connection, database);
    }
  }

  /// <summary>
  /// Factory for creating mock Redis stream publishers and subscribers for testing purposes.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the identity.</typeparam>
  public static class MockRedisStreamFactory<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : class, IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    /// <summary>
    /// Creates a mock publisher.
    /// </summary>
    /// <returns>A mock publisher.</returns>
    public static Mock<IPublisher<TEntity, TEvent, TIdentityType>> CreatePublisher()
    {
      return new Mock<IPublisher<TEntity, TEvent, TIdentityType>>();
    }

    /// <summary>
    /// Creates a mock subscriber and database.
    /// </summary>
    /// <param name="collection">An optional collection of entities to initialize the subscriber with.</param>
    /// <returns>A tuple containing the mock subscriber and database.</returns>
    public static (Mock<RedisStreamSubscriber<TEntity, TEvent, TIdentityType>> Subscriber, Mock<IDatabase> Database) CreateSubscriber(IEnumerable<TEntity>? collection = null)
    {
      var (connection, database) = MockRedisStreamFactory.CreateMockConnection();
      var mockSubscriber = new Mock<RedisStreamSubscriber<TEntity, TEvent, TIdentityType>>(connection.Object, new RedisSubscriberOptions());
      SetupMockSubscriberCollection(mockSubscriber, collection);
      return (mockSubscriber, database);
    }

    /// <summary>
    /// Sets up the mock subscriber with a collection of entities.
    /// </summary>
    /// <typeparam name="TSubscriberType">The type of the subscriber.</typeparam>
    /// <param name="mockSubscriber">The mock subscriber.</param>
    /// <param name="collection">An optional collection of entities to initialize the subscriber with.</param>
    public static void SetupMockSubscriberCollection<TSubscriberType>(Mock<TSubscriberType> mockSubscriber, IEnumerable<TEntity>? collection = null)
      where TSubscriberType : RedisStreamSubscriber<TEntity, TEvent, TIdentityType>
    {
      if (collection != null)
      {
        _ = mockSubscriber
          .Setup(t => t.GetMessagesAsync(It.IsAny<int>()))
          .ReturnsAsync(ExpandWithRedisValues(collection));
        SetupSupportMockMethods(mockSubscriber, collection);
      }
    }

    /// <summary>
    /// Sets up support methods for the mock subscriber.
    /// </summary>
    /// <typeparam name="TSubscriberType">The type of the subscriber.</typeparam>
    /// <param name="mockSubscriber">The mock subscriber.</param>
    /// <param name="collection">An optional collection of entities to initialize the subscriber with.</param>
    public static void SetupSupportMockMethods<TSubscriberType>(Mock<TSubscriberType> mockSubscriber, IEnumerable<TEntity>? collection)
      where TSubscriberType : RedisStreamSubscriber<TEntity, TEvent, TIdentityType>
    {
      _ = mockSubscriber
        .Setup(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()))
        .ReturnsAsync(1);
      _ = mockSubscriber
        .Setup(t => t.GetStreamInfoAsync())
        .ReturnsAsync(new MessageQueueInfo
        {
          DeadLetterMsgCount = 0,
          AcknowledgedMsgCount = 0,
          PendingMsgCount = 0,
          MsgCount = collection?.Count(),
          SubscriberCount = 1,
        });
    }

    /// <summary>
    /// Expands a collection of entities with Redis values.
    /// </summary>
    /// <param name="collection">The collection of entities.</param>
    /// <returns>A collection of tuples containing Redis values and entities.</returns>
    public static IEnumerable<(RedisValue Id, TEntity Entity)> ExpandWithRedisValues(IEnumerable<TEntity>? collection = null)
    {
      return collection?.Select(t => (new RedisValue(t.Id.ToString()), t));
    }
  }
}
