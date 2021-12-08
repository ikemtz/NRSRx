using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace IkeMtz.Samples.OData.Tests.Integration
{
  [TestClass]
  public partial class SchoolsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetItemsTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>());
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true");
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(envelope.Count, envelope.Value.Count());
      envelope.Value.ToList().ForEach(t =>
      {
        Assert.IsNotNull(t.Name);
        Assert.AreNotEqual(Guid.Empty, t.Id);
      });
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByItemsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
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
      HttpResponseMessage resp = null;
      try
      {
        resp = await client.GetAsync($"odata/v1/{nameof(School)}s?$apply=groupby(({nameof(item.Name)},{nameof(item.TenantId)}))");
      }
      catch (Exception) { }
      var body = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {body}");
      Assert.IsFalse(body.ToLower().Contains("updatedby"));
      StringAssert.Contains(body, item.Name);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetItemsWithExpansionsTest()
    {
      var school = Factories.SchoolFactory();
      var course = Factories.CourseFactory();
      school.SchoolCourses.Add(Factories.SchoolCourseFactory(school, course));
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
        $"odata/v1/{nameof(School)}s?$filter=id eq {school.Id}&$expand={nameof(school.SchoolCourses)},{nameof(school.StudentSchools)}");
      TestContext.WriteLine($"Server Reponse: {resp}");

      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      Assert.AreEqual(1, envelope.Value.First().SchoolCourses.Count);
      StringAssert.Contains(resp, school.Name);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("SqlIntegration")]
    public async Task GetGroupByItemsTestWithAggregations()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationODataTestStartup>()
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

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$apply=aggregate(id with countdistinct as total)");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      StringAssert.Contains(resp, "total");
    }
  }
}
