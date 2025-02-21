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

namespace IkeMtz.NRSRx.Logging.Elasticsearch.Tests
{
  [TestClass]
  public class ElasticsearchLoggingTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ConsoleLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      var startup = new StartUp_Elastic(moqConfiguration.Object);
      var result = startup.SetupConsoleLogging(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ElasticLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      _ = moqConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
      var startup = new StartUp_Elastic(moqConfiguration.Object);
      var result = startup.SetupElasticsearch(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void UseLoggingTest()
    {
      var moq = new Mock<IHostBuilder>();
      var result = SeriLogExtensions.UseLogging(moq.Object);
      Assert.IsNotNull(result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CreateDefaultHostBuilderWithElasticTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<StartUp_Elastic>().UseLogging();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();

      var loggerFac = host.Services.GetService<ILoggerFactory>();
      Assert.IsNotNull(loggerFac);
      var logger = loggerFac.CreateLogger("Unit Test");
      Assert.IsNotNull(logger);
      logger.LogError("Validating Error logging");
      await host.StopAsync();
      await host.WaitForShutdownAsync();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetSwaggerUI()
    {
      using var srv = new TestServer(TestWebHostBuilder<StartUp_Elastic, StartUp_Elastic>());
      var client = srv.CreateClient(TestContext);
      var resp = await client.GetAsync("index.html");
      Assert.AreEqual(HttpStatusCode.OK, resp.EnsureSuccessStatusCode().StatusCode);
    }
  }
}
