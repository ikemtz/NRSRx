using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Abstraction.Redis
{
  /// <summary>
  /// Core class for handling Redis streams for a specific entity and event type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the entity identifier.</typeparam>
  public class RedisStreamCore<TEntity, TEvent, TIdentityType>
      where TIdentityType : IComparable
      where TEntity : IIdentifiable<TIdentityType>
      where TEvent : EventType, new()
  {
    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    protected string TypeName { get; }

    /// <summary>
    /// Gets the Redis connection multiplexer.
    /// </summary>
    public IConnectionMultiplexer Connection { get; }

    /// <summary>
    /// Gets the Redis database.
    /// </summary>
    public IDatabase Database { get; }

    /// <summary>
    /// Gets the Redis stream key.
    /// </summary>
    public RedisKey StreamKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamCore{TEntity, TEvent, TIdentityType}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    public RedisStreamCore(IConnectionMultiplexer connection)
    {
      EntityType = typeof(TEntity);
      TypeName = EntityType.IsGenericType ? $"{EntityType.GenericTypeArguments[0].Name}:{EntityType.Name.Split("`").First()}" : EntityType.Name;
      Connection = connection;
      Database = connection.GetDatabase();
      var eventType = new TEvent();
      StreamKey = $"{TypeName}:{eventType.EventSuffix}";
    }
  }
}
