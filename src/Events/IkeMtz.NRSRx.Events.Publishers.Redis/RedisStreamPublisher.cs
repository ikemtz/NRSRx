using System;
using System.Text;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Newtonsoft.Json;
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
      ISimplePublisher<TEntity, TEvent, RedisValue, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    public IConnectionMultiplexer Connection { get; }
    public IDatabase Database { get; }
    public RedisKey StreamKey { get; }

    public RedisStreamPublisher(IConnectionMultiplexer connection)
    {
      Connection = connection;
      Database = connection.GetDatabase();
      var eventType = new TEvent();
      StreamKey = $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
    public virtual async Task<RedisValue> PublishAsync(TEntity payload)
    {
      var json = JsonConvert.SerializeObject(payload);
      var plainTextBytes = Encoding.UTF8.GetBytes(json);
      var base64 = System.Convert.ToBase64String(plainTextBytes);
      return await Database.StreamAddAsync(StreamKey,
           new RedisValue(payload.Id.ToString()), new RedisValue(base64));
    }
  }
}
