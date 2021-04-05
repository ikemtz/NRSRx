using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class ItemDeletedPublisher : RedisStreamPublisher<Item, DeletedEvent>,
    ISimplePublisher<Item, DeletedEvent, RedisValue>
  {
    public ItemDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
