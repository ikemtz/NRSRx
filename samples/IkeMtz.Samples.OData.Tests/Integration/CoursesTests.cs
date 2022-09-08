using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Integration
{
  [TestClass]
  public partial class CoursesTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetCoursesTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s?$count=true");
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.AreEqual(envelope?.Count, envelope?.Value.Count());
      envelope?.Value.ToList().ForEach(t =>
      {
        Assert.IsNotNull(t.Num);
        Assert.AreNotEqual(Guid.Empty, t.Id);
      });
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByCoursesTest()
    {
      var Course = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(Course);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());
      var resp = await client.GetAsync($"odata/v1/{nameof(Course)}s?$apply=groupby(({nameof(Course.Num)},{nameof(Course.Title)}))");
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.IsFalse(body.ToLower().Contains("updatedby"));
      StringAssert.Contains(body, Course.Num);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetCoursesWithExpansionsTest()
    {
      var school = Factories.SchoolFactory();
      var course = Factories.CourseFactory();
      course.SchoolCourses.Add(Factories.SchoolCourseFactory(school, course));
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(school);
              _ = db.Courses.Add(course);
              db.SchoolCourses.AddRange(school.SchoolCourses);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync(
        $"odata/v1/{nameof(Course)}s?$filter=id eq {course.Id}&$expand={nameof(course.SchoolCourses)},{nameof(course.StudentCourses)}");
      TestContext.WriteLine($"Server Reponse: {resp}");

      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      Assert.AreEqual(1, envelope?.Value.First().SchoolCourses.Count);
      StringAssert.Contains(resp, course.Num);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByCoursesTestWithAggregations()
    {
      var Course = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(Course);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s?$apply=aggregate(id with countdistinct as total)");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      StringAssert.Contains(resp, "total");
    }
  }
}
