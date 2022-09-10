using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  [TestClass]
  public partial class SchoolsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSchoolsTest()
    {
      var objA = Factories.SchoolFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Schools.Add(objA);
            });
          })
       );
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(School)}s?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(resp);
      Assert.AreEqual(objA.Name, envelope?.Value.First().Name);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteSchoolTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"odata/v1/{nameof(School)}s/{Guid.NewGuid()}");
      var content = await resp.Content.ReadAsStringAsync();
      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {content}");
      _ = resp.EnsureSuccessStatusCode();
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<School>>(content);
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}
