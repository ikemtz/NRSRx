using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Swagger;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task TestJsonDocAsync()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnitTestStartup>());
      var doc = await SwaggerUnitTests.TestJsonDocAsync(srv);
      Assert.IsNotNull(doc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task TestHtmlPageAsync()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnitTestStartup>());
      var html = await SwaggerUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }
  }
}
