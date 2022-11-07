using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis.Publishers
{
  public class CourseCreatedPublisher : RedisStreamPublisher<Course, CreatedEvent>,
    IPublisher<Course, CreatedEvent>
  {
    public CourseCreatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class CourseDeletedPublisher : RedisStreamPublisher<Course, DeletedEvent>,
    IPublisher<Course, DeletedEvent>
  {
    public CourseDeletedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
  public class CourseUpdatedPublisher : RedisStreamPublisher<Course, UpdatedEvent>,
    IPublisher<Course, UpdatedEvent>
  {
    public CourseUpdatedPublisher(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
