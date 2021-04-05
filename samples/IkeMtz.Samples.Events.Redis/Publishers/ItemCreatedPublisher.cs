using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class ItemCreatedPublisher : RedisStreamPublisher<Item, CreatedEvent>,
    ISimplePublisher<Item, CreatedEvent, RedisValue>
  {
    public ItemCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
