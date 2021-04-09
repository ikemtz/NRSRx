using System;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Integration
{
  [TestClass]
  public partial class ItemsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetItemsTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Item)}s?$count=true");
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Item>>(resp);
      Assert.AreEqual(envelope.Count, envelope.Value.Count());
      envelope.Value.ToList().ForEach(t =>
      {
        Assert.IsNotNull(t.Value);
        Assert.AreNotEqual(Guid.Empty, t.Id);
      });
    }
  }
}
