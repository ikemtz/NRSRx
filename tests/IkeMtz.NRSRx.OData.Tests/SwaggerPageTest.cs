using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class SwaggerPageTest : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSwaggerPageTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();

      var resp = await client.GetAsync("index.html");
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      StringAssert.Contains(body, "<base href=\"/\">");
    }
  }
}
