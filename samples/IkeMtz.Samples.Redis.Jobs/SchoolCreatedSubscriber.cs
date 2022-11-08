using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using StackExchange.Redis;

namespace IkeMtz.Samples.Redis.Jobs
{
  public class SchoolCreatedSubscriber : RedisStreamSubscriber<School, CreatedEvent>
  {
    public SchoolCreatedSubscriber(IConnectionMultiplexer connection) : base(connection)
    {
    }
  }
}
