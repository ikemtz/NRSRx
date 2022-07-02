using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Http;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class SchoolTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSchoolTestAsync()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Schools.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("MyTestClaim", Guid.NewGuid().ToString()) }));
      //Get 
      var resp = await client.GetAsync($"api/v1/{nameof(School)}s.json?id={item.Id}");
      var httpSchool = await DeserializeResponseAsync<School>(resp);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.AreEqual(item.Name, httpSchool.Name);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SaveSchoolTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(School)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      var httpSchool = await DeserializeResponseAsync<School>(resp);
      Assert.AreEqual("IntegrationTester@email.com", httpSchool.CreatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbSchools = await dbContext.Schools.ToListAsync();

      Assert.AreEqual(1, dbSchools.Count);
      var dbSchool = dbSchools.FirstOrDefault();
      Assert.IsNotNull(dbSchool);
      Assert.AreEqual(httpSchool.CreatedOnUtc, dbSchool.CreatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    [ExpectedException(typeof(JsonReaderException))]
    public async Task SaveSchoolJsonReaderExceptionsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(School)}s.xml", item);
      _ = resp.EnsureSuccessStatusCode();
      _ = await DeserializeResponseAsync<School>(resp);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateSchoolTest()
    {
      var originalSchool = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Schools.Add(originalSchool);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var updatedSchool = JsonClone(originalSchool);
      updatedSchool.Name = Guid.NewGuid().ToString()[..6];

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(School)}s.json?id={updatedSchool.Id}", updatedSchool);
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedSchool = await DeserializeResponseAsync<School>(resp);
      Assert.AreEqual("IntegrationTester@email.com", httpUpdatedSchool.UpdatedBy);
      Assert.AreEqual(updatedSchool.Name, httpUpdatedSchool.Name);
      Assert.IsNull(updatedSchool.UpdatedOnUtc);
      Assert.IsNotNull(httpUpdatedSchool.UpdatedOnUtc);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbSchools = await dbContext.Schools.ToListAsync();

      Assert.AreEqual(1, dbSchools.Count);
      var updatedDbSchool = dbSchools.FirstOrDefault();
      Assert.IsNotNull(updatedDbSchool);
      Assert.IsNotNull(updatedDbSchool.UpdatedOnUtc);
      Assert.AreEqual(httpUpdatedSchool.UpdatedOnUtc, updatedDbSchool.UpdatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteSchoolTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Schools.Add(item);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(School)}s.json?id={item.Id}");
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedSchool = await DeserializeResponseAsync<School>(resp);
      Assert.IsNull(httpUpdatedSchool.UpdatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbSchools = await dbContext.Schools.ToListAsync();

      Assert.AreEqual(0, dbSchools.Count);
    }
  }
}
