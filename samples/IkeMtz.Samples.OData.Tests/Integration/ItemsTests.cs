using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.OData.Data;
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
    [TestCategory("SqlIntegration")]
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

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByItemsTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
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
      HttpResponseMessage resp = null;
      try
      {
        resp = await client.GetAsync($"odata/v1/{nameof(Item)}s?$apply=groupby(({nameof(item.Value)},{nameof(item.TenantId)}))");
      }
      catch (Exception) { }
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.IsFalse(body.ToLower().Contains("updatedby"));
      StringAssert.Contains(body, item.Value);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetItemsWithExpansionsTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
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

      var resp = await client.GetStringAsync(
        $"odata/v1/{nameof(Item)}s?$expand=subItemAs,subItemBs,subItemCs");
      TestContext.WriteLine($"Server Reponse: {resp}");

      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Item>>(resp);
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      Assert.AreEqual(1, envelope.Value.First().SubItemAs.Count);
      StringAssert.Contains(resp, item.Value);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByItemsTestWithAggregations()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
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

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Item)}s?$apply=aggregate(id with countdistinct as total)");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      StringAssert.StartsWith(resp, "[{\"total\":"); 
    }
  }
}
