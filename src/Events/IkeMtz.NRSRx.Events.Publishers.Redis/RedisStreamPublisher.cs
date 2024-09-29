using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis
{
  /// <summary>
  /// Redis stream publisher for entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public class RedisStreamPublisher<TEntity, TEvent> :
       RedisStreamPublisher<TEntity, TEvent, Guid>,
        IPublisher<TEntity, TEvent>
     where TEntity : IIdentifiable<Guid>
     where TEvent : EventType, new()
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamPublisher{TEntity, TEvent}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }

  /// <summary>
  /// Redis stream publisher for entities.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the entity identifier.</typeparam>
  public class RedisStreamPublisher<TEntity, TEvent, TIdentityType> :
      RedisStreamCore<TEntity, TEvent, TIdentityType>,
      IPublisher<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    /// <summary>
    /// Gets or sets the name of the entity.
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamPublisher{TEntity, TEvent, TIdentityType}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
      var type = typeof(TEntity);
      if (type.IsGenericType)
      {
        type = type.GenericTypeArguments[0];
      }
      EntityName = type.Name;
    }

    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <param name="payload">The entity payload.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public virtual Task PublishAsync(TEntity payload)
    {
      return Database.StreamAddAsync(StreamKey,
           new RedisValue(EntityName), new RedisValue(JsonConvert.SerializeObject(payload, Constants.JsonSerializerSettings)));
    }
  }
}
