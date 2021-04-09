using System;
using IkeMtz.NRSRx.Core.Models;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Abstraction.Redis
{
  public class RedisStreamCore<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    public IConnectionMultiplexer Connection { get; }
    public IDatabase Database { get; }
    public RedisKey StreamKey { get; }
    public RedisStreamCore(IConnectionMultiplexer connection)
    {
      Connection = connection;
      Database = connection.GetDatabase();
      var eventType = new TEvent();
      StreamKey = $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
  }
}
