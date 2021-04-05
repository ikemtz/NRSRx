using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class ItemUpdatedPublisher : RedisStreamPublisher<Item, UpdatedEvent>,
    ISimplePublisher<Item, UpdatedEvent, RedisValue>
  {
    public ItemUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
