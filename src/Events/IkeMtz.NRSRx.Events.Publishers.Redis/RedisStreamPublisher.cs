using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis
{
  public class RedisStreamPublisher<TEntity, TEvent> :
     RedisStreamPublisher<TEntity, TEvent, Guid>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }

  public class RedisStreamPublisher<TEntity, TEvent, TIdentityType> :
      RedisStreamCore<TEntity, TEvent, TIdentityType>,
      ISimplePublisher<TEntity, TEvent, RedisValue, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {

    public RedisStreamPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
    public virtual async Task<RedisValue> PublishAsync(TEntity payload)
    {
      var base64 = Convert.ToBase64String(MessageCoder.JsonEncode(payload));
      return await Database.StreamAddAsync(StreamKey,
           new RedisValue(payload.Id.ToString()), new RedisValue(base64));
    }
  }
}
