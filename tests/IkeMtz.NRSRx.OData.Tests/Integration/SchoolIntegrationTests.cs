using System;
using System.Linq;
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
  public class SchoolIntegrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task GetSchoolsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$filter=id eq {item.Id}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.Contains("updatedby", StringComparison.CurrentCultureIgnoreCase));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(item.Name, envelope?.Value.First().Name);
    }


    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task GetItemsWithExpansionTest()
    {
      var item = Factories.SchoolFactory();
      var student = Factories.StudentFactory();
      var course = Factories.CourseFactory();
      _ = Factories.SchoolCourseFactory(item, course);
      _ = Factories.StudentSchoolFactory(student, item);
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(item);
              _ = db.Courses.Add(course);
              _ = db.Students.Add(student);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true&$expand={nameof(item.SchoolCourses)},{nameof(item.StudentSchools)}&$filter=id eq {item.Id}");

      Assert.IsFalse(resp.Contains("updatedby", StringComparison.CurrentCultureIgnoreCase));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.Name, envelope.Value.First().Name);
      Assert.AreEqual(item.SchoolCourses.First().Id, envelope.Value.First().SchoolCourses.First().Id);
      Assert.AreEqual(item.StudentSchools.First().Id, envelope.Value.First().StudentSchools.First().Id);
    }
  }
}
