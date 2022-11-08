using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class StudentCreatedPublisher : RedisStreamPublisher<Student, CreatedEvent>,
    IPublisher<Student, CreatedEvent>
  {
    public StudentCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class StudentDeletedPublisher : RedisStreamPublisher<Student, DeletedEvent>,
    IPublisher<Student, DeletedEvent>
  {
    public StudentDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class StudentUpdatedPublisher : RedisStreamPublisher<Student, UpdatedEvent>,
    IPublisher<Student, UpdatedEvent>
  {
    public StudentUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
