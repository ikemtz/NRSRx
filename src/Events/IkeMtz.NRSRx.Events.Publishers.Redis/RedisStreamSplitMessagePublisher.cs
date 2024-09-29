using System;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis
{
  /// <summary>
  /// Redis stream publisher for split messages of entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public class RedisStreamSplitMessagePublisher<TEntity, TEvent> :
       RedisStreamPublisher<SplitMessage<TEntity>, TEvent, Guid>,
        IPublisher<SplitMessage<TEntity>, TEvent>
     where TEntity : IIdentifiable<Guid>
     where TEvent : EventType, new()
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamSplitMessagePublisher{TEntity, TEvent}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    public RedisStreamSplitMessagePublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
