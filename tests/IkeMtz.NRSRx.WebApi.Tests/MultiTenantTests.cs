using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Tests;
using IkeMtz.Samples.Models;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class MultiTenantTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMultiTenantItemTestAsync()
    {
      var item = Factories.ItemFactory();
      item.TenantId = "a-b-c";
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Items.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("tids", item.TenantId) }));

      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(Item)}s.json?id={item.Id}&tid={item.TenantId}");
      var httpItem = await DeserializeResponseAsync<Item>(resp);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.AreEqual(item.Value, httpItem.Value);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoTenantsTestAsync()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(Item)}s.json?id={Guid.NewGuid()}&tid=xyz");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
      Assert.AreEqual("The current user doesn't have access to any tenants.", result);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoAccessToTenantTestAsync()
    {
      var item = Factories.ItemFactory();
      item.TenantId = "a-b-c";
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Items.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("tids", item.TenantId) }));
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(Item)}s.json?id={item.Id}&tid={item.TenantId}x");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
      Assert.AreEqual("The current user doesn't have access to the a-b-cx tenant.", result);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoTenantParameterTestAsync()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(Item)}s.json?id={Guid.NewGuid()}");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
      Assert.AreEqual("Query string param tid is required for this endpoint.", result);
    }
  }
}
