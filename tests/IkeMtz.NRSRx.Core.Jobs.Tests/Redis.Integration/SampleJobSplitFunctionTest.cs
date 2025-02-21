using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Redis.Integration
{
  [TestClass]
  public class SampleJobSplitFunctionTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("RedisIntegration")]
    public async Task SampleJobSplitFunctionTest()
    {
      //arange
      var program = new IntegrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
        SecsBetweenRuns = 10,
      };
      _ = program.SetupHost();

      var subscriber = program.JobHost.Services.GetRequiredService<RedisStreamSubscriber<SplitMessage<School>, UpdatedEvent>>();
      var originalStreamInfo = await subscriber.GetStreamInfoAsync(); // Need to establish a baseline

      var publisher = new RedisStreamPublisher<SplitMessage<School>, UpdatedEvent>(program.Program.RedisConnectionMultiplexer);

      foreach (var schoolSplit in SplitMessageFactory<School>.Create(Factories.SchoolFactory, 10))
      {
        await publisher.PublishAsync(schoolSplit);
      }


      //act
      var currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreEqual(10, currentStreamInfo.MsgCount - originalStreamInfo.MsgCount); //Ensuring published messages worked

      await program.RunAsync();

      //assert
      currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfo.MsgCount);
      Assert.AreNotEqual(0, currentStreamInfo.AcknowledgedMsgCount);
      Assert.AreEqual(currentStreamInfo.MsgCount, currentStreamInfo.AcknowledgedMsgCount);
      Assert.AreEqual(0, currentStreamInfo.DeadLetterMsgCount);
      _ = await subscriber.DeleteIdleConsumersAsync();
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.StreamKey);
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.ConsumerGroupAckCounterKey);
    }

  }
}
