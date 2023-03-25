using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.NRSRx.Jobs.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Jobs.Redis.Tests.Unigration
{
    [TestClass]
    public class SampleSplitFunctionTests : BaseUnigrationTests
    {
        [TestMethod]
        [TestCategory("Unigration")]
        public async Task SampleSplitFunctionTest()
        {
            //arange
            var program = new SplitMessageUnigrationProgram(new SplitProgram(), TestContext)
            {
                RunContinously = false,
            };
            _ = program.SetupHost(x =>
               x.AddSingleton<SplitSchoolCreatedFunction>());

            //act
            await program.RunAsync();

            //assert
            program.MockSubscriber.Verify(t => t.GetMessagesAsync(It.IsAny<int?>()), Times.Once);
            program.MockSubscriber.Verify(t => t.GetStreamInfoAsync(), Times.Exactly(1));
            program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Exactly(2));
        }

        [TestMethod]
        [TestCategory("Unigration")]
        public async Task SampleSchoolCreatedEventTest()
        {
            //arange
            var program = new SplitMessageUnigrationProgram(new SplitProgram(), TestContext)
            {
                RunContinously = false,
                SecsBetweenRuns = 1,
                EnableParallelFunctionProcessing = true,
            };

            _ = program.SetupHost(x => x.AddSingleton<SplitSchoolCreatedFunction>());
            var func = program.JobHost.Services.GetRequiredService<SplitSchoolCreatedFunction>();
            var logger = program.JobHost.Services.GetRequiredService<ILogger<SplitSchoolCreatedFunction>>();

            //act
            await func.HandleMessageAsync(new SplitMessage<School>(Factories.SchoolFactory()));

            //assert
            Assert.AreEqual(1, program.SleepTimeSpan.TotalSeconds);
            program.MockSubscriber.Verify(t => t.GetMessagesAsync(It.Is<int>(x => x == 5)), Times.Never);
            program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Never);
            program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Never);

        }
    }

    internal class SplitSchoolCreatedFunction : SplitMessageFunction<SplitSchoolCreatedFunction, School, CreatedEvent>
    {
        public SplitSchoolCreatedFunction(ILogger<SplitSchoolCreatedFunction> logger, RedisStreamSubscriber<SplitMessage<School>, CreatedEvent> subscriber)
          : base(logger, subscriber)
        {
            this.MessageBufferCount = 1;
        }
        public override Task HandleMessageAsync(SplitMessage<School> entity)
        {
            return Task.CompletedTask;
        }
    }

    internal class SplitMessageUnigrationProgram : CoreMessagingUnigrationTestJob<SplitProgram>
    {
        public Mock<RedisStreamSubscriber<SplitMessage<School>, CreatedEvent>> MockSubscriber { get; set; }
        public SplitMessageUnigrationProgram(SplitProgram program, TestContext testContext) : base(program, testContext)
        {
        }

        public override IServiceCollection SetupDependencies(IServiceCollection services)
        {
            var schools = new[] {
        new SplitMessage<School>(Factories.SchoolFactory()){        TaskIndex = 1,        TaskCount=2},
        new SplitMessage<School>(Factories.SchoolFactory()){TaskIndex =1, TaskCount =2},
      };
            var (Subscriber, _) = MockRedisStreamFactory<SplitMessage<School>, CreatedEvent>.CreateSubscriber(schools);
            MockSubscriber = Subscriber;
            return services.AddSingleton(x => MockSubscriber.Object);
        }
    }
    public class SplitProgram : MessagingJob<SplitProgram>
    {
        public override IServiceCollection SetupFunctions(IServiceCollection services)
        {
            return services.AddSingleton<IMessageFunction, SplitSchoolCreatedFunction>();
        }
    }
}
