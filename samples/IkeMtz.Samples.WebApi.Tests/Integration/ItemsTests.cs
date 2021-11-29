using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Tests;
using IkeMtz.Samples.WebApi.Data;
using IkeMtz.Samples.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.WebApi.Tests.Integration
{
  [TestClass]
  public partial class ItemsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task SaveItemsTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationWebApiTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Item)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      var httpItem = await DeserializeResponseAsync<Item>(resp);
      Assert.AreEqual("IntegrationTester@email.com", httpItem.CreatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbItem = await dbContext.Items.FirstOrDefaultAsync(t => t.Id == item.Id);

      Assert.IsNotNull(dbItem);
      Assert.AreEqual(httpItem.CreatedOnUtc, dbItem.CreatedOnUtc);
    }


    [TestMethod]
    [TestCategory("Integration")]
    public async Task UpdateItemTest()
    {
      var originalItem = Factories.ItemFactory();
      originalItem.CreatedBy = "blah";
      originalItem.CreatedOnUtc = DateTime.UtcNow;
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationWebApiTestStartup>()
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
      updatedItem.Value = Guid.NewGuid().ToString()[..6];

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Item)}s.json?id={updatedItem.Id}", updatedItem);
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedItem = await DeserializeResponseAsync<Item>(resp);
      Assert.AreEqual("IntegrationTester@email.com", httpUpdatedItem.UpdatedBy);
      Assert.AreEqual(updatedItem.Value, httpUpdatedItem.Value);
      Assert.IsNull(updatedItem.UpdatedOnUtc);
      Assert.IsNotNull(httpUpdatedItem.UpdatedOnUtc);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var updatedDbItem = await dbContext.Items.FirstOrDefaultAsync(t => t.Id == originalItem.Id);

      Assert.IsNotNull(updatedDbItem);
      Assert.IsNotNull(updatedDbItem.UpdatedOnUtc);
      Assert.AreEqual(httpUpdatedItem.UpdatedOnUtc, updatedDbItem.UpdatedOnUtc);
    }

  }
}
