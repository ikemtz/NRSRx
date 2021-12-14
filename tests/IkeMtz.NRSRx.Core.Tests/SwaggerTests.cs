using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Swagger;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task TestJsonDocAsync()
    {
      using var srv = new TestServer(TestHostBuilder<StartUp_AppInsights, UnitTestStartup>());
      var doc = await SwaggerUnitTests.TestJsonDocAsync(srv);
      _ = await SwaggerUnitTests.TestReverseProxyJsonDocAsync(srv);
      Assert.IsNotNull(doc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task TestHtmlPageAsync()
    {
      using var srv = new TestServer(TestHostBuilder<StartUp_AppInsights, UnitTestStartup>());
      var html = await SwaggerUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void TestGetSwaggerScopes()
    {
      var result = ConfigureSwaggerOptions.GetSwaggerScopeDictionary(new[] {
        new OAuthScope("A", "X"),
        new OAuthScope("A", "B"),
        new OAuthScope("B", "B"),
        new OAuthScope("B", "Z"),
        new OAuthScope("C", "Y"),
      });
      Assert.AreEqual(3, result.Count);
    }
  }
}
