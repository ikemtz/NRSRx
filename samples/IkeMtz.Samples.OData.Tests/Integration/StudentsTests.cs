using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Integration
{
  [TestClass]
  public partial class StudentsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetStudentsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s?$count=true");
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.AreEqual(envelope?.Count, envelope?.Value.Count());
      envelope?.Value.ToList().ForEach(t =>
      {
        Assert.IsNotNull(t.FirstName);
        Assert.AreNotEqual(Guid.Empty, t.Id);
      });
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByStudentsTest()
    {
      var Student = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(Student);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());
      var resp = await client.GetAsync($"odata/v1/{nameof(Student)}s?$apply=groupby(({nameof(Student.FirstName)},{nameof(Student.BirthDate)}))");
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.IsFalse(body.ToLower().Contains("updatedby"));
      StringAssert.Contains(body, Student.FirstName);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetStudentsWithExpansionsTest()
    {
      var student = Factories.StudentFactory();
      var course = Factories.CourseFactory();
      var school = Factories.SchoolFactory();
      school.SchoolCourses.Add(Factories.SchoolCourseFactory(school, course));
      student.StudentCourses.Add(Factories.StudentCourseFactory(student, school.SchoolCourses.First()));
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(student);
              _ = db.Courses.Add(course);
              _ = db.Schools.Add(school);
              db.SchoolCourses.AddRange(school.SchoolCourses);
              db.StudentCourses.AddRange(student.StudentCourses);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync(
        $"odata/v1/{nameof(Student)}s?$filter=id eq {student.Id}&$expand={nameof(student.StudentCourses)},{nameof(student.StudentSchools)}");
      TestContext.WriteLine($"Server Reponse: {resp}");

      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Student>>(resp);
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      Assert.AreEqual(1, envelope?.Value.First().StudentCourses.Count);
      StringAssert.Contains(resp, student.FirstName);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByStudentsTestWithAggregations()
    {
      var Student = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Students.Add(Student);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Student)}s?$apply=aggregate(id with countdistinct as total)");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      StringAssert.Contains(resp, "total");
    }
  }
}
