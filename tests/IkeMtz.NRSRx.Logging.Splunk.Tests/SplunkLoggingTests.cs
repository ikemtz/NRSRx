using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.Logging.Splunk.Tests
{
  [TestClass]
  public class SplunkLoggingTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ConsoleLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      _ = moqConfiguration.SetupGet(s => s["DisableSwagger"]).Returns(true.ToString);
      var startup = new StartUp_Splunk(moqConfiguration.Object);
      var result = startup.SetupConsoleLogging(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void SplunkLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      _ = moqConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
      var startup = new StartUp_Splunk(moqConfiguration.Object);
      var result = startup.SetupSplunk(null);
      Assert.IsNotNull(result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CreateDefaultHostBuilderWithSplunkTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<StartUp_Splunk>().UseLogging();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();
      var loggerFac = host.Services.GetService<ILoggerFactory>();
      Assert.IsNotNull(loggerFac);
      var logger = loggerFac.CreateLogger("Unit Test");
      logger.LogError("Validating Error logging");
      await host.StopAsync();
      await host.WaitForShutdownAsync();
      Assert.IsNotNull(logger);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetSwaggerUI()
    {
      using var srv = new TestServer(TestWebHostBuilder<StartUp_Splunk, StartUp_Splunk>());
      var client = srv.CreateClient(TestContext);
      var resp = await client.GetAsync("index.html");
      Assert.AreEqual(HttpStatusCode.OK, resp.EnsureSuccessStatusCode().StatusCode);
    }
  }
}
