using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Redis.Integration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RedisIntegration")]
    public async Task SampleJobTest()
    {
      //arange
      var program = new IntegrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
        SecsBetweenRuns = 10,
      };
      _ = program.SetupHost();

      var subscriber = program.JobHost.Services.GetRequiredService<RedisStreamSubscriber<School, CreatedEvent>>();
      var originalStreamInfo = await subscriber.GetStreamInfoAsync(); // Need to establish a baseline

      var publisher = new RedisStreamPublisher<School, CreatedEvent>(program.Program.RedisConnectionMultiplexer);
      await publisher.PublishAsync(Factories.SchoolFactory());
      await publisher.PublishAsync(Factories.SchoolFactory());

      //act
      var currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreEqual(2, currentStreamInfo.MessageCount - originalStreamInfo.MessageCount); //Ensuring published messages worked

      await program.RunAsync();

      //assert
      currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfo.MessageCount);
      Assert.AreNotEqual(0, currentStreamInfo.AckMessageCount);
      Assert.AreEqual(currentStreamInfo.MessageCount, currentStreamInfo.AckMessageCount);
      Assert.AreEqual(1, currentStreamInfo.SubscriberCount - originalStreamInfo.SubscriberCount);
      Assert.AreEqual(0, currentStreamInfo.DeadLetterCount);
      _ = await subscriber.DeleteIdleConsumersAsync();
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.StreamKey);
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.ConsumerGroupCounterKey);
    }
  }
}
