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
  public partial class SchoolsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("SqlIntegration")]
    public async Task GetSchoolsTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true");
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(envelope?.Count, envelope?.Value.Count());
      envelope?.Value.ToList().ForEach(t =>
      {
        Assert.IsNotNull(t.Name);
        Assert.AreNotEqual(Guid.Empty, t.Id);
      });
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupBySchoolsTest()
    {
      var School = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
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
      var resp = await client.GetAsync($"odata/v1/{nameof(School)}s?$apply=groupby(({nameof(School.Name)},{nameof(School.TenantId)}))");
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.DoesNotContain("updatedby", body.ToLower());
      Assert.Contains(School.Name, body);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("SqlIntegration")]
    public async Task GetSchoolsWithExpansionsTest()
    {
      var school = Factories.SchoolFactory();
      var course = Factories.CourseFactory();
      school.SchoolCourses.Add(Factories.SchoolCourseFactory(school, course));
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
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
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync(
        $"odata/v1/{nameof(School)}s?$filter=id eq {school.Id}&$expand={nameof(school.SchoolCourses)},{nameof(school.StudentSchools)}");
      TestContext.WriteLine($"Server Reponse: {resp}");

      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.DoesNotContain("updatedby", resp.ToLower());
      Assert.AreEqual(1, envelope?.Value.First().SchoolCourses.Count);
      Assert.Contains(school.Name, resp);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupBySchoolsTestWithAggregations()
    {
      var School = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationODataTestStartup>()
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

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$apply=aggregate(id with countdistinct as total)");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.DoesNotContain("updatedby", resp.ToLower());
      Assert.Contains("total", resp);
    }
  }
}
