using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Swagger;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task TestJsonDocAsync()
    {
      using var srv = new TestServer(TestWebHostBuilder<StartUp_AppInsights, UnitTestStartup>());
      var doc = await SwaggerUnitTests.TestJsonDocAsync(srv);
      Assert.IsNotNull(doc);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task TestHtmlPageAsync()
    {
      using var srv = new TestServer(TestWebHostBuilder<StartUp_AppInsights, UnitTestStartup>());
      var html = await SwaggerUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void TestGetSwaggerScopes()
    {
      //var result = ConfigureSwaggerOptions.GetSwaggerScopeDictionary([
      //  new OAuthScope("A", "X"),
      //  new OAuthScope("A", "B"),
      //  new OAuthScope("B", "B"),
      //  new OAuthScope("B", "Z"),
      //  new OAuthScope("C", "Y"),
      //]);
      //Assert.HasCount(3, result);
    }
  }
}
