using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class SchoolUnigrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSchoolsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(item);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(item.Name, envelope.Value.First().Name);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetGroupBySchoolsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(item);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$apply=groupby(({nameof(item.Name)},{nameof(item.Id)}),aggregate(id with countdistinct as total))");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      StringAssert.Contains(resp, item.Id.ToString());
      StringAssert.Contains(resp, item.Name);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSchoolsWithExpansionTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(item);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true&$expand={nameof(item.SchoolCourses)},{nameof(item.StudentSchools)}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(item.Name, envelope.Value.First().Name);
      Assert.AreEqual(item.SchoolCourses.First().Id, envelope.Value.First().SchoolCourses.First().Id);
      Assert.AreEqual(item.StudentSchools.First().Id, envelope.Value.First().StudentSchools.First().Id);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMaxErrorItemsTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s?$top=5000&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.IsTrue(data.Contains("The limit of '100'"));
      Assert.IsTrue(data.Contains("The value from the incoming request is '5000'"));
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMaxItemsTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s/nolimit?$top=500&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteItemsTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"odata/v1/{nameof(School)}s/{Guid.NewGuid()}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    [Ignore] //NOSONAR
    public async Task GetMetaData()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient();
      var resp = await client.GetAsync($"$odata");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}
