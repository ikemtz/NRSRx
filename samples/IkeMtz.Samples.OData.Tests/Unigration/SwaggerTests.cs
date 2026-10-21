using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.OpenApi;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerIndexPageTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>());
      var html = await OpenApiUnitTests.TestHtmlPageAsync(srv, cancellationToken: TestContext.CancellationToken);
      Assert.IsNotNull(html);
    }

    [TestMethod, Timeout(Timespans.ThirtySeconds, CooperativeCancellation = false)]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerJsonTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>());
      var doc = await OpenApiUnitTests.TestJsonDocAsync(srv, new Startup(null), cancellationToken: TestContext.CancellationToken);
      Assert.IsTrue(doc.Components.Schemas.ContainsKey(nameof(School)));
      Assert.Contains(a => a.Key.Contains(GetODataEnvelopeName<School>()), doc.Components.Schemas);
      Assert.AreEqual($"{nameof(Samples)} OData Microservice", doc.Info.Title);
      Assert.AreEqual(12, doc.Components?.Schemas?.Count);
    }
  }
}
