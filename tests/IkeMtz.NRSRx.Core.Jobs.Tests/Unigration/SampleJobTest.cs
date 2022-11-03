using IkeMtz.NRSRx.Core.Unigration;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  [TestClass]
  public class SampleJobTest : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task Testjob()
    {
      var program = new UnigrationProgram(TestContext);
      await program.RunAsync();
    }
  }
}
