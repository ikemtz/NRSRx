using System;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis
{
  public class RedisStreamSplitMessagePublisher<TEntity, TEvent> :
     RedisStreamPublisher<SplitMessage<TEntity>, TEvent, Guid>,
      IPublisher<SplitMessage<TEntity>, TEvent>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public RedisStreamSplitMessagePublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
