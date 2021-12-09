using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class CourseCreatedPublisher : RedisStreamPublisher<Course, CreatedEvent>,
    ISimplePublisher<Course, CreatedEvent, RedisValue>
  {
    public CourseCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class CourseDeletedPublisher : RedisStreamPublisher<Course, DeletedEvent>,
    ISimplePublisher<Course, DeletedEvent, RedisValue>
  {
    public CourseDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class CourseUpdatedPublisher : RedisStreamPublisher<Course, UpdatedEvent>,
    ISimplePublisher<Course, UpdatedEvent, RedisValue>
  {
    public CourseUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
