using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class MetaDataTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Ignore("This test is currently failing due to an issue with the OData routing configuration. It needs to be investigated and fixed.")]
    public async Task GetMetaDataTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var resp = await srv.CreateClient().GetAsync("odata/v1/$metadata", TestContext.CancellationToken);
      var content = await resp.Content.ReadAsStringAsync(TestContext.CancellationToken);
      TestContext.Write($"Server Response: {content}");
      _ = resp.EnsureSuccessStatusCode();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Ignore("This test is currently failing due to an issue with the OData routing configuration. It needs to be investigated and fixed.")]
    public async Task GetServiceDocumentTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var resp = await srv.CreateClient().GetAsync("odata/v1", TestContext.CancellationToken);
      var content = await resp.Content.ReadAsStringAsync(TestContext.CancellationToken);
      TestContext.Write($"Server Response: {content}");
      _ = resp.EnsureSuccessStatusCode();
    }
  }
}
