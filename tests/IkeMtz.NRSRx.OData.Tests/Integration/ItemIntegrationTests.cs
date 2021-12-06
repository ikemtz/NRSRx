using System.Linq;
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
  public class SchoolIntegrationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetSchoolsTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationTestStartup>()
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

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$filter=id eq {item.Id}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(item.Name, envelope.Value.First().Name);
    }


    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetItemsWithExpansionTest()
    {
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, IntegrationTestStartup>()
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

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true&$expand={nameof(item.SchoolCourses)},{nameof(item.StudentSchools)}&$filter=id eq {item.Id}");
      TestContext.WriteLine($"Server Reponse: {resp}");
      Assert.IsFalse(resp.ToLower().Contains("updatedby"));
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(item.Name, envelope.Value.First().Name);
      Assert.AreEqual(item.SchoolCourses.First().Id, envelope.Value.First().SchoolCourses.First().Id);
      Assert.AreEqual(item.StudentSchools.First().Id, envelope.Value.First().StudentSchools.First().Id);
    }
  }
}
