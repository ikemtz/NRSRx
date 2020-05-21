using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using IkeMtz.Samples.WebApi.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class ItemTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetItemTestAsync()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Items.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("MyTestClaim", Guid.NewGuid().ToString()) }));
      //Get 
      var resp = await client.GetAsync($"api/v1/{nameof(Item)}s.json?id={item.Id}");
      var httpItem = await DeserializeResponseAsync<Item>(resp);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.AreEqual(item.Value, httpItem.Value);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SaveItemTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Item)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      var httpItem = await DeserializeResponseAsync<Item>(resp);
      Assert.AreEqual("Integration Tester", httpItem.CreatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbItems = await dbContext.Items.ToListAsync();

      Assert.AreEqual(1, dbItems.Count);
      var dbItem = dbItems.FirstOrDefault();
      Assert.IsNotNull(dbItem);
      Assert.AreEqual(httpItem.CreatedOnUtc, dbItem.CreatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateItemTest()
    {
      var originalItem = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Items.Add(originalItem);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var updatedItem = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(originalItem));
      updatedItem.Value = Guid.NewGuid().ToString().Substring(0, 6);

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Item)}s.json?id={updatedItem.Id}", updatedItem);
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedItem = await DeserializeResponseAsync<Item>(resp);
      Assert.AreEqual("Integration Tester", httpUpdatedItem.UpdatedBy);
      Assert.AreEqual(updatedItem.Value, httpUpdatedItem.Value);
      Assert.IsNull(updatedItem.UpdatedOnUtc);
      Assert.IsNotNull(httpUpdatedItem.UpdatedOnUtc);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbItems = await dbContext.Items.ToListAsync();

      Assert.AreEqual(1, dbItems.Count);
      var updatedDbItem = dbItems.FirstOrDefault();
      Assert.IsNotNull(updatedDbItem);
      Assert.IsNotNull(updatedDbItem.UpdatedOnUtc);
      Assert.AreEqual(httpUpdatedItem.UpdatedOnUtc, updatedDbItem.UpdatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteItemTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Items.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(Item)}s.json?id={item.Id}");
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedItem = await DeserializeResponseAsync<Item>(resp);
      Assert.IsNull(httpUpdatedItem.UpdatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbItems = await dbContext.Items.ToListAsync();

      Assert.AreEqual(0, dbItems.Count);
    }
  }
}
