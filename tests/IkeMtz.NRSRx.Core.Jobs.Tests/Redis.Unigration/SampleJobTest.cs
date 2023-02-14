using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Jobs.Redis.Tests.Unigration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SampleJobTest()
    {
      //arange
      var program = new UnigrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
      };

      //act
      await program.RunAsync();

      //assert
      program.MockSubscriber.Verify(t => t.GetMessagesAsync(It.IsAny<int?>()), Times.Once);
      program.MockSubscriber.Verify(t => t.GetStreamInfoAsync(), Times.Exactly(3));
      program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Exactly(2));
    }


    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SampleSchoolCreatedEventTest()
    {
      //arange
      var program = new UnigrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
        SecsBetweenRuns = 1,
      };

      _ = program.SetupHost(x => x.AddSingleton<SchoolCreatedFunction>());
      var func = program.JobHost.Services.GetRequiredService<SchoolCreatedFunction>();
      var logger = program.JobHost.Services.GetRequiredService<ILogger<SchoolCreatedFunction>>();

      //act
      await func.HandleMessageAsync(Factories.SchoolFactory());

      //assert
      Assert.AreEqual(1, program.SleepTimeSpan.TotalSeconds);
      program.MockSubscriber.Verify(t => t.GetMessagesAsync(It.Is<int>(x => x == 5)), Times.Never);
      program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Never);

    }
  }
}
