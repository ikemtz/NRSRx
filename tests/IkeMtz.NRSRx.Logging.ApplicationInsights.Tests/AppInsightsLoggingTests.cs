using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.Logging.ApplicationInsights.Tests
{
  [TestClass]
  [DoNotParallelize]
  public class AppInsightsLoggingTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void TestUseLogging()
    {
      var moq = new Mock<IHostBuilder>();
      var result = moq.Object.UseLogging();
      Assert.AreEqual(moq.Object, result);
    }
  }
}
