using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class HealthPageTest : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetHealthPageTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var cli = srv.CreateClient();
      var resp = await cli.GetStringAsync("/healthz");
      Assert.AreEqual("Healthy", resp);
    }
  }
}
