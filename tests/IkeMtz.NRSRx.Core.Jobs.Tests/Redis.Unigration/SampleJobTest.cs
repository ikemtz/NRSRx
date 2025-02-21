using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
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
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
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
      program.SchoolCreatedSubMock.Verify(t => t.GetMessagesAsync(It.IsAny<int?>()), Times.Once);
      program.SchoolCreatedSubMock.Verify(t => t.GetStreamInfoAsync(), Times.Exactly(1));
      program.SchoolCreatedSubMock.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Exactly(2));
    }


    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task SampleSchoolCreatedEventTest()
    {
      //arange
      var program = new UnigrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
        SecsBetweenRuns = 1,
        EnableParallelFunctionProcessing = true,
      };

      _ = program.SetupHost(x => x.AddSingleton<SchoolCreatedFunction>());
      var func = program.JobHost.Services.GetRequiredService<SchoolCreatedFunction>();
      var logger = program.JobHost.Services.GetRequiredService<ILogger<SchoolCreatedFunction>>();

      //act
      await func.HandleMessageAsync(Factories.SchoolFactory());

      //assert
      Assert.AreEqual(1, program.SleepTimeSpan.TotalSeconds);
      program.SchoolCreatedSubMock.Verify(t => t.GetMessagesAsync(It.Is<int>(x => x == 5)), Times.Never);
      program.SchoolCreatedSubMock.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Never);

    }
  }
}
