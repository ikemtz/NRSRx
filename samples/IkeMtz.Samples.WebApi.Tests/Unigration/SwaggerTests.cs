using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.OpenApi;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.WebApi.Tests.Unigration
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerIndexPageTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationWebApiTestStartup>());
      var html = await OpenApiUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerJsonTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationWebApiTestStartup>());
      var doc = await OpenApiUnitTests.TestJsonDocAsync(srv);
      Assert.IsTrue(doc.Components.Schemas.ContainsKey(nameof(Course)));
      Assert.AreEqual($"{nameof(Samples)} WebApi Microservice", doc.Info.Title);
    }
  }
}
