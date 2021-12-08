using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  [TestClass]
  public partial class CoursesTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetCoursesTest()
    {
      var objA = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Courses.Add(objA);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Course)}s?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Course>>(resp);
      Assert.AreEqual(objA.Title, envelope.Value.First().Title);
    }
  }
}
