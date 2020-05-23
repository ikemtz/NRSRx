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
    public async Task CreateDefaultHostBuilderTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<Startup>();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();
      await host.StopAsync();
      await host.WaitForShutdownAsync();
    }
  }
}
