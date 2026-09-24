using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Controllers.V1;
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
    [TestCategory(TestCategories.Unigration)]
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

      var resp = await client.GetStringAsync($"{GetFullRoute<StudentsController>()}?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Response: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.AreEqual(objA.FirstName, envelope?.Value.First().FirstName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
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

      var resp = await client.GetAsync($"{GetFullRoute<StudentsController>()}/noLimit?$count=true&$top=500");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Response: {content}");
      _ = resp.EnsureSuccessStatusCode();
      var envelope = JsonConvert.DeserializeObject<Student[]>(content);
      Assert.AreEqual(objA.FirstName, envelope?.First().FirstName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
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

      var resp = await client.GetAsync($"{GetFullRoute<StudentsController>()}?$count=true&$top=300");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Response: {content}");
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
      Assert.Contains("The limit of '100' for Top query has been exceeded. The value from the incoming request is '300'.", content);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task DeleteStudentTest()
    {
      var studentEntity = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(studentEntity);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"{GetFullRoute<StudentsController>()}/{studentEntity.Id}");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Response: {content}");
      _ = resp.EnsureSuccessStatusCode();
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(content);
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}
