using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IkeMtz.NRSRx.Core.Jobs.Redis.Tests.Unigration
{
  internal class UnigrationProgram : CoreMessagingUnigrationTestJob<Program>
  {
    public Mock<RedisStreamSubscriber<School, CreatedEvent>> MockSubscriber { get; set; }
    public UnigrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
    }

    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      var schools = new[] {
        Factories.SchoolFactory(),
        Factories.SchoolFactory(),
      };
      var (Subscriber, _) = MockRedisStreamFactory<School, CreatedEvent>.CreateSubscriber(schools);
      MockSubscriber = Subscriber;
      return services.AddSingleton(x => MockSubscriber.Object);
    }
  }
}
