using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis
{
  public class RedisStreamPublisher<TEntity, TEvent> :
     RedisStreamPublisher<TEntity, TEvent, Guid>,
      IPublisher<TEntity, TEvent>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }

  public class RedisStreamPublisher<TEntity, TEvent, TIdentityType> :
      RedisStreamCore<TEntity, TEvent, TIdentityType>,
      IPublisher<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {

    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
    public virtual Task PublishAsync(TEntity payload)
    {
      return Database.StreamAddAsync(StreamKey,
           new RedisValue(payload.Id.ToString()), new RedisValue(JsonConvert.SerializeObject(payload)));
    }
  }
}
