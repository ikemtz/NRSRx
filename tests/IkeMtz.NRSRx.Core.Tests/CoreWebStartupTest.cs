using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class CoreWebStartupTest : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateDefaultHostBuilderWithAppInsightsTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<StartUp_AppInsights>();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();
      await host.StopAsync();
      await host.WaitForShutdownAsync();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateDefaultHostBuilderWithSplunkTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<StartUp_Splunk>();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();
      await host.StopAsync();
      await host.WaitForShutdownAsync();
    }
  }
}
