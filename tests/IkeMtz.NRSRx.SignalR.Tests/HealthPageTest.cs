using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.SignalR.Tests;
using IkeMtz.Samples.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class HealthPageTest : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetHealthPageTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var cli = srv.CreateClient();
      var resp = await cli.GetStringAsync("/health");
      Assert.AreEqual("Healthy", resp);
    }
  }
}
