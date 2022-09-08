using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Data;
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
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(objA);
            });
          })
       );
      var client = srv.CreateClient();
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
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(objA);
            });
          })
       );
      var client = srv.CreateClient();
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
    public async Task DeleteStudentTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>());
      var client = srv.CreateClient();
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
