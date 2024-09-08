using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using IkeMtz.Samples.WebApi;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class MultiTenantTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMultiTenantSchoolTestAsync()
    {
      var item = Factories.SchoolFactory();
      item.TenantId = "a-b-c";
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Schools.Add(item);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("tids", item.TenantId) }));

      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(School)}s.json?id={item.Id}&tid={item.TenantId}");
      var httpSchool = await DeserializeResponseAsync<School>(resp);
      Assert.IsNotNull(httpSchool);
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.AreEqual(item.Name, httpSchool.Name);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoTenantsTestAsync()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(School)}s.json?id={Guid.NewGuid()}&tid=xyz");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
      Assert.AreEqual("The current user doesn't have access to any tenants.", result);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoAccessToTenantTestAsync()
    {
      var item = Factories.SchoolFactory();
      item.TenantId = "a-b-c";
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Schools.Add(item);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("tids", item.TenantId) }));
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(School)}s.json?id={item.Id}&tid={item.TenantId}x");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.Unauthorized, resp.StatusCode);
      Assert.AreEqual("The current user doesn't have access to the a-b-cx tenant.", result);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task NoTenantParameterTestAsync()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());
      //Get 
      var resp = await client.GetAsync($"api/v1/MultiTenant{nameof(School)}s.json?id={Guid.NewGuid()}");
      var result = await resp.Content.ReadAsStringAsync();

      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
      Assert.AreEqual("Query string param tid is required for this endpoint.", result);
    }
  }
}
