using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.Samples.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;

namespace IkeMtz.NRSRx.Logging.Elastisearch.Tests
{
  [TestClass]
  public class ElastisearchLoggingTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void ConsoleLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      var startup = new Startup(moqConfiguration.Object);
      var result = startup.SetupConsoleLogging();
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void ElasticLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      moqConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
      var startup = new Startup(moqConfiguration.Object);
      var result = startup.SetupElastisearch();
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
  }
}
