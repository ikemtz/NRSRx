using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class StudentUnigrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetStudentsTest()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.DoesNotContain("updatedby", resp.ToLower());
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.FirstName, envelope.Value.First().FirstName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetGroupByStudentsTest()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s?$apply=groupby(({nameof(item.FirstName)},{nameof(item.Id)}),aggregate(id with countdistinct as total))");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.DoesNotContain("updatedby", resp.ToLower());
      Assert.Contains(item.Id.ToString(), resp);
      Assert.Contains(item.FirstName, resp);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetStudentsWithExpansionTest()
    {
      var item = Factories.StudentCourseFactory(Factories.StudentFactory(), Factories.CourseFactory(), Factories.SchoolFactory());
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(item.Student);
              _ = db.Courses.Add(item.Course);
              _ = db.StudentCourses.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s?$count=true&$expand={nameof(StudentCourse)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.DoesNotContain("updatedby", resp.ToLower());
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.Student.FirstName, envelope.Value.First().FirstName);
      Assert.AreEqual(item.Id, envelope.Value.First().StudentCourses.First().Id);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetMaxErrorStudentsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Student)}s?$top=5000&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.Contains("The limit of '100'", data);
      Assert.Contains("The value from the incoming request is '5000'", data);
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetMaxStudentsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Student)}s/nolimit?$top=500&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task DeleteStudentsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"odata/v1/{nameof(Student)}s/{Guid.NewGuid()}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Ignore("waiting for fix: https://github.com/OData/AspNetCoreOData/issues/420")]
    public async Task GetODataDebugPage()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      var resp = await client.GetAsync($"$odata");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);

      SnapshotAsserter.AssertEachLineIsEqual(SnapShotResources.ODataDebugPage, data);
    }
  }
}

