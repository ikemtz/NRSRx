using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class SchoolCreatedPublisher : RedisStreamPublisher<School, CreatedEvent>,
    IPublisher<School, CreatedEvent>
  {
    public SchoolCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class SchoolDeletedPublisher : RedisStreamPublisher<School, DeletedEvent>,
    IPublisher<School, DeletedEvent>
  {
    public SchoolDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class SchoolUpdatedPublisher : RedisStreamPublisher<School, UpdatedEvent>,
    IPublisher<School, UpdatedEvent>
  {
    public SchoolUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
