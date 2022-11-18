using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Jobs.Redis.Tests.Unigration
{
  internal class UnigrationProgram : CoreMessagingJobUnigrationTestProgram<Program>
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
      }.Select(t => (new RedisValue(t.Id.ToString()), t));
      var (Subscriber, _) = MockRedisStreamFactory<School, CreatedEvent>.CreateSubscriber();
      MockSubscriber = Subscriber;
      _ = MockSubscriber
        .Setup(t => t.GetStreamInfoAsync())
        .ReturnsAsync(new MessageQueueInfo
        {
          DeadLetterCount = 5,
          MessageCount = 10,
          SubscriberCount = 2,
        });
      _ = MockSubscriber
            .Setup(t => t.GetMessagesAsync(It.Is<int>(x => x == 5)))
            .ReturnsAsync(schools);
      _ = MockSubscriber
            .Setup(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()))
            .ReturnsAsync(1);

      return services.AddSingleton(x => MockSubscriber.Object);
    }
  }
}
