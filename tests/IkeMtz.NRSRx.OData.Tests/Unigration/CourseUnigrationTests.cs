using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData;
using IkeMtz.Samples.OData.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class CourseUnigrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetCoursesTest()
    {
      var item = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.Title, envelope.Value.First().Title);
    }
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetCoursesWithInterceptorTest()
    {
      var item = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.Title, envelope.Value.First().Title);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetGroupByCoursesTest()
    {
      var item = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Course)}s?$orderby=title&$apply=groupby(({nameof(item.Title)},{nameof(item.Id)}),aggregate({nameof(item.Id)} with countdistinct as total,{nameof(item.PassRate)} with sum as sumPassRate,{nameof(item.AvgScore)} with max as maxScore))&$count=true");
      var content = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(content.ToLower().Contains("updatedby"));
      StringAssert.Contains(content, item.Id.ToString());
      StringAssert.Contains(content, item.Title);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetCoursesWithExpansionTest()
    {
      var item = Factories.SchoolCourseFactory(Factories.SchoolFactory(), Factories.CourseFactory());
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(item.Course);
              _ = db.Schools.Add(item.School);
              _ = db.SchoolCourses.Add(item);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s?$count=true&$expand={nameof(SchoolCourse)}s");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.IsNotNull(envelope);
      Assert.AreEqual(item.Course.Title, envelope.Value.First().Title);
      Assert.AreEqual(item.Id, envelope.Value.First().SchoolCourses.First().Id);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMaxErrorCoursesTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetAsync($"odata/v1/{nameof(Course)}s?$top=5000&$count=true");
      var data = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Server Reponse: {data}");
      Assert.IsTrue(data.Contains("The limit of '100'"));
      Assert.IsTrue(data.Contains("The value from the incoming request is '5000'"));
      Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
    }
  }
}
