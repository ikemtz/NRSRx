using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Abstraction.Redis
{
  public class RedisStreamCore<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    public Type EntityType { get; }
    protected string TypeName { get; }
    public IConnectionMultiplexer Connection { get; }
    public IDatabase Database { get; }
    public RedisKey StreamKey { get; }
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
