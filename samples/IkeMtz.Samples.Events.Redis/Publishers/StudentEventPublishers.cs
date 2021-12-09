using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class StudentCreatedPublisher : RedisStreamPublisher<Student, CreatedEvent>,
    ISimplePublisher<Student, CreatedEvent, RedisValue>
  {
    public StudentCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class StudentDeletedPublisher : RedisStreamPublisher<Student, DeletedEvent>,
    ISimplePublisher<Student, DeletedEvent, RedisValue>
  {
    public StudentDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class StudentUpdatedPublisher : RedisStreamPublisher<Student, UpdatedEvent>,
    ISimplePublisher<Student, UpdatedEvent, RedisValue>
  {
    public StudentUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
