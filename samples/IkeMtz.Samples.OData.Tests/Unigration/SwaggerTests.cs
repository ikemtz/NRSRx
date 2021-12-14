using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Swagger;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSwaggerIndexPageTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>());
      var html = await SwaggerUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSwaggerJsonTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>());
      var doc = await SwaggerUnitTests.TestJsonDocAsync(srv);
      _ = await SwaggerUnitTests.TestReverseProxyJsonDocAsync(srv);

      Assert.IsTrue(doc.Components.Schemas.ContainsKey(nameof(School)));
      Assert.IsTrue(doc.Components.Schemas.Any(a => a.Key.Contains("SchoolGuidODataEnvelope")));
      Assert.AreEqual($"{nameof(Samples)} OData Microservice", doc.Info.Title);
    }
  }
}
