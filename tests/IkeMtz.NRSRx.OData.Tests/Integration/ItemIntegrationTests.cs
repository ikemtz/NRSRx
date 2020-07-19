using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.OData.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class ItemIntegrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetItemsTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Items.Add(item);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Item)}s.json?$filter=id eq {item.Id}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Item>>(resp);
      Assert.AreEqual(item.Value, envelope.Value.First().Value);
    }


    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetItemsWithExpansionTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Items.Add(item);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Item)}s.json?$count=true&$expand={nameof(SubItemA)}s,{nameof(SubItemB)}s&$filter=id eq {item.Id}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Item>>(resp);
      Assert.AreEqual(item.Value, envelope.Value.First().Value);
      Assert.AreEqual(item.SubItemAs.First().Id, envelope.Value.First().SubItemAs.First().Id);
      Assert.AreEqual(item.SubItemBs.First().Id, envelope.Value.First().SubItemBs.First().Id);
      Assert.AreEqual(item.SubItemBs.First().Id, envelope.Value.First().SubItemBs.First().Id);
      Assert.IsFalse(envelope.Value.First().SubItemCs.Any());
    }
  }
}
