using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Redis.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Redis.Integration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
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

      var subscriber = program.JobHost.Services.GetRequiredService<RedisStreamSubscriber<School, CreateEvent>>();
      var originalStreamInfo = await subscriber.GetStreamInfoAsync(); // Need to establish a baseline

      var publisher = new RedisStreamPublisher<School, CreateEvent>(program.Program.RedisConnectionMultiplexer);
      await publisher.PublishAsync(Factories.SchoolFactory());
      await publisher.PublishAsync(Factories.SchoolFactory());

      //act
      var currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreEqual(2, currentStreamInfo.MsgCount - originalStreamInfo.MsgCount); //Ensuring published messages worked

      await program.RunAsync();

      //assert
      currentStreamInfo = await subscriber.GetStreamInfoAsync();
      Assert.AreNotEqual(0, currentStreamInfo.MsgCount);
      Assert.AreNotEqual(0, currentStreamInfo.AcknowledgedMsgCount);
      Assert.AreEqual(currentStreamInfo.MsgCount, currentStreamInfo.AcknowledgedMsgCount);
      Assert.AreEqual(1, currentStreamInfo.SubscriberCount - originalStreamInfo.SubscriberCount);
      Assert.AreEqual(0, currentStreamInfo.DeadLetterMsgCount);
      _ = await subscriber.DeleteIdleConsumersAsync();
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.StreamKey);
      _ = await subscriber.Database.KeyDeleteAsync(subscriber.ConsumerGroupAckCounterKey);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("RedisIntegration")]
    public async Task RedisStreamSplitMessagePublisherTest()
    {
      var program = new IntegrationProgram(new Program(), TestContext)
      {
        RunContinously = false,
        SecsBetweenRuns = 10,
      };
      _ = program.SetupHost();

      var redisConnectionString = program.Configuration.GetValue<string>("REDIS_CONNECTION_STRING");
      var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

      var publisher = new RedisStreamSplitMessagePublisher<School, CreatedEvent>(connectionMultiplexer);
      foreach (var splitMessage in SplitMessageFactory<School>.Create(Factories.SchoolFactory))
      {
        await publisher.PublishAsync(splitMessage);
      }
      //assert
      Assert.IsNotNull(publisher);
    }
  }
}
