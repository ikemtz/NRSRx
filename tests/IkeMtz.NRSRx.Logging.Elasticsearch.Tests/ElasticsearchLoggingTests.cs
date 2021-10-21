using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.Logging.Elasticsearch.Tests
{
  [TestClass]
  public class ElasticsearchLoggingTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void ConsoleLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      var startup = new StartUp_Elastic(moqConfiguration.Object);
      var result = startup.SetupConsoleLogging(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void ElasticLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      moqConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
      var startup = new StartUp_Elastic(moqConfiguration.Object);
      var result = startup.SetupElasticsearch(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void UseLoggingTest()
    {
      var moq = new Mock<IHostBuilder>();
      var result = SeriLogExtensions.UseLogging(moq.Object);
      Assert.IsNotNull(result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateDefaultHostBuilderWithElasticTest()
    {
      var builder = CoreWebStartup.CreateDefaultHostBuilder<StartUp_Elastic>().UseLogging();
      var host = builder.Build();
      Assert.IsNotNull(host);
      await host.StartAsync();

      var loggerFac = host.Services.GetService<ILoggerFactory>();
      var logger = loggerFac.CreateLogger("Unit Test");
      logger.LogError("Validating Error logging");
      await host.StopAsync();
      await host.WaitForShutdownAsync();
    }
  }
}
