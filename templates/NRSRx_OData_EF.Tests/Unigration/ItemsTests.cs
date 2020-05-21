using System;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NRSRx_OData_EF;
using NRSRx_OData_EF.Data;
using NRSRx_OData_EF.Models;

namespace NRSRx_OData_EF.Tests.Unigration
{
  [TestClass]
  public partial class ItemsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetItemsTest()
    {
      var objA = new Item()
      {
        Id = Guid.NewGuid(),
        Value = $"Test- {Guid.NewGuid().ToString().Substring(29)}",
      };
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.Items.Add(objA);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(Item)}s?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<Item>>(resp);
      Assert.AreEqual(objA.Value, envelope.Value.First().Value);
    }
  }
}
