using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  [TestClass]
  public partial class StudentsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetStudentsTest()
    {
      var objA = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(objA);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.AreEqual(objA.FirstName, envelope?.Value.First().FirstName);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetStudentsWNoLimitTest()
    {
      var objA = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(objA);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Student)}s/noLimit?$count=true&$top=500");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {content}");
      _ = resp.EnsureSuccessStatusCode();
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(content);
      Assert.AreEqual(objA.FirstName, envelope?.Value.First().FirstName);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetStudentsExceedLimitTest()
    {
      var objA = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(objA);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Student)}s?$count=true&$top=300");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {content}");
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
      StringAssert.Contains(content, "The limit of '100' for Top query has been exceeded. The value from the incoming request is '300'.");
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteStudentTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"odata/v1/{nameof(Student)}s/{Guid.NewGuid()}");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {content}");
      _ = resp.EnsureSuccessStatusCode();
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(content);
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}
