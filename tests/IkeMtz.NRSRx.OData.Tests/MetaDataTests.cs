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
    [TestCategory("Unigration")]
    [Ignore] //NOSONAR
    public async Task GetMetaDataTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, Startup>());
      var resp = await srv.CreateClient().GetAsync("odata/v1");
      var content = await resp.Content.ReadAsStringAsync();
      TestContext.Write($"Server Response: {content}");
      _ = resp.EnsureSuccessStatusCode();
    }
  }
}
