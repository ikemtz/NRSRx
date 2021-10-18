using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.Samples.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.Logging.Elastisearch.Tests
{
  [TestClass]
  public class SplunkLoggingTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void ConsoleLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      var startup = new Startup(moqConfiguration.Object);
      var result = startup.SetupConsoleLogging(null);
      Assert.IsNotNull(result);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void ElasticLoggingTest()
    {
      var moqConfiguration = new Mock<IConfiguration>();
      moqConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(new Mock<IConfigurationSection>().Object);
      var startup = new Startup(moqConfiguration.Object);
      var result = startup.SetupSplunk(null);
      Assert.IsNotNull(result);
    }
  }
}
