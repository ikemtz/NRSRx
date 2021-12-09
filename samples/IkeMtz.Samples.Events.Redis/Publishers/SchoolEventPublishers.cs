using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class SchoolCreatedPublisher : RedisStreamPublisher<School, CreatedEvent>,
    ISimplePublisher<School, CreatedEvent, RedisValue>
  {
    public SchoolCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class SchoolDeletedPublisher : RedisStreamPublisher<School, DeletedEvent>,
    ISimplePublisher<School, DeletedEvent, RedisValue>
  {
    public SchoolDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class SchoolUpdatedPublisher : RedisStreamPublisher<School, UpdatedEvent>,
    ISimplePublisher<School, UpdatedEvent, RedisValue>
  {
    public SchoolUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
