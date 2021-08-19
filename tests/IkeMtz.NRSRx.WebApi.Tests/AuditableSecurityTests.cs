using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Tests;
using IkeMtz.Samples.Models;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class AuditableSecurityTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    [ExpectedException(typeof(AuditableInvalidUserException))]
    public async Task SaveItemCausesAuditInvalidUserExTest()
    {
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      _ = await client.PostAsJsonAsync($"api/v1/{nameof(Item)}s.json", item);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    [ExpectedException(typeof(AuditableInvalidUserException))]
    public async Task UpdateItemCausesAuditInvalidUserExTest()
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
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      var updatedItem = JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(originalItem));
      updatedItem.Value = Guid.NewGuid().ToString().Substring(0, 6);

      _ = await client.PutAsJsonAsync($"api/v1/{nameof(Item)}s.json?id={updatedItem.Id}", updatedItem);
    }

  }
}
