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
  public class SchoolUnigrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSchoolsTest()
    {
      var School = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(School);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.Contains("updatedby", StringComparison.CurrentCultureIgnoreCase));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(School.Name, envelope.Value.First().Name);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetGroupBySchoolsTest()
    {
      var School = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(School);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$apply=groupby(({nameof(School.Name)},{nameof(School.Id)}),aggregate(id with countdistinct as total))");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.Contains("updatedby", System.StringComparison.CurrentCultureIgnoreCase));
      Assert.Contains(School.Id.ToString(), resp);
      Assert.Contains(School.Name, resp);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSchoolsWithExpansionTest()
    {
      var schoolCourse = Factories.SchoolCourseFactory(Factories.SchoolFactory(), Factories.CourseFactory());
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(schoolCourse.School);
              _ = db.Courses.Add(schoolCourse.Course);
              _ = db.SchoolCourses.Add(schoolCourse);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true&$expand={nameof(schoolCourse)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.Contains("updatedby", System.StringComparison.CurrentCultureIgnoreCase));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(schoolCourse.School.Name, envelope.Value.First().Name);
      Assert.AreEqual(schoolCourse.Id, envelope.Value.First().SchoolCourses.First().Id);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetMaxErrorSchoolsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s?$top=5000&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.Contains("The limit of '100'", data);
      Assert.Contains("The value from the incoming request is '5000'", data);
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
    }


    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Ignore("Waiting for fix: https://github.com/OData/AspNetCoreOData/issues/420")]
    public async Task ComputeMinSchoolTest()
    {
      var school = Factories.SchoolFactory();
      var schoolCourseA = Factories.SchoolCourseFactory(school, Factories.CourseFactory());
      var schoolCourseB = Factories.SchoolCourseFactory(school, Factories.CourseFactory());
      var schoolCourseC = Factories.SchoolCourseFactory(school, Factories.CourseFactory());
      var schoolCourseD = Factories.SchoolCourseFactory(school, Factories.CourseFactory());
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(school);
              _ = db.SchoolCourses.Add(schoolCourseA);
              _ = db.SchoolCourses.Add(schoolCourseB);
              _ = db.SchoolCourses.Add(schoolCourseC);
              _ = db.SchoolCourses.Add(schoolCourseD);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s?$top=100&$count=true&$compute={nameof(school.SchoolCourses)}/$count as Courses&$select={nameof(school.Id)},Courses");
      var data = await resp.Content.ReadAsStringAsync();
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(data);
      Assert.Contains($"\"id\":\"{school.Id}\",\"Courses\":4", data);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Ignore("waiting for fix: https://github.com/OData/AspNetCoreOData/issues/420")]
    public async Task GetMaxSchoolsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s/nolimit?$top=500&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task DeleteSchoolsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"odata/v1/{nameof(School)}s/{Guid.NewGuid()}");
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}
