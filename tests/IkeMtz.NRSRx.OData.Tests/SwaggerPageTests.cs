using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.OpenApi;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  [DoNotParallelize]
  public class SwaggerPageTests : BaseUnigrationTests
  {
    [TestMethod, Timeout(Timespans.ThirtySeconds, CooperativeCancellation = false)]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerPageTest()
    {
      Startup.DisableSwagger = null;
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var htmlPage = await OpenApiUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(htmlPage);
      var jsonDoc = await OpenApiUnitTests.TestJsonDocAsync(srv, new Startup(null), cancellationToken: TestContext.CancellationToken);
      Assert.IsNotNull(jsonDoc);
      Assert.AreEqual(12, jsonDoc.Components?.Schemas?.Count);
    }

    [TestMethod, Timeout(Timespans.ThirtySeconds, CooperativeCancellation = false)]
    [TestCategory(TestCategories.Unigration)]
    public async Task GetSwaggerJsonTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());
      var doc = await OpenApiUnitTests.TestJsonDocAsync(srv, new Startup(null), cancellationToken: TestContext.CancellationToken);
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(Course)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(School)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(Student)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(CourseStatus)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(Gender)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(SchoolCourse)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(StudentCourse)));
      Assert.IsTrue(doc.Components?.Schemas?.ContainsKey(nameof(StudentSchool)));

      Assert.AreEqual($"{nameof(Samples)} OData Microservice", doc.Info.Title);
      Assert.AreEqual(12, doc.Components?.Schemas?.Count);
      var entities = new List<string>
      {
        nameof(CourseStatus),
        nameof(Gender),

        nameof(Course),
        nameof(School),
        nameof(Student),
        nameof(SchoolCourse),
        nameof(StudentSchool),
        nameof(StudentCourse),
      };
      foreach (var entity in entities)
      {
        Assert.IsTrue(doc.Components?.Schemas.ContainsKey(entity));
      }
      Assert.AreEqual(3,
        doc.Components?.Schemas.Keys.Where(t => t.StartsWith("ODataEnvelopeOf")).Count());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ValidateODataEnvelopName()
    {
      var result = new BaseUnigrationTests().GetODataEnvelopeName<School>();
      Assert.AreEqual("ODataEnvelopeOfSchoolAndGuid", result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task DisableSwaggerPageTest()
    {
      Startup.DisableSwagger = null;
      var myConfiguration = new Dictionary<string, string?>
      {
        {"DisableSwagger", "true"},
      };
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureAppConfiguration((builderContext, configurationBuilder) =>
          configurationBuilder.AddInMemoryCollection(myConfiguration)
        ));

      var client = srv.CreateClient();
      // Get the OpenAPI HTML page
      var htmlResponse = await client.GetAsync($"index.html").ConfigureAwait(true);
      Assert.AreEqual(HttpStatusCode.NotFound, htmlResponse.StatusCode);

      var JsonResponse = await client.GetAsync($"/openapi/v1.json").ConfigureAwait(true);
      Assert.AreEqual(HttpStatusCode.NotFound, JsonResponse.StatusCode);
    }
  }
}
