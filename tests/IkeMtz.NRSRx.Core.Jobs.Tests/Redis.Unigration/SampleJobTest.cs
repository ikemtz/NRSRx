using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Redis.Jobs;
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
      program.MockSubscriber.Verify(t => t.GetMessagesAsync(It.Is<int>(x => x == 5)), Times.Once);
      program.MockSubscriber.Verify(t => t.AcknowledgeMessageAsync(It.IsAny<RedisValue>()), Times.Exactly(2));
    }
  }
}
