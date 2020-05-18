using System.Threading.Tasks;
using System.Xml;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.OData.Tests
{
  [TestClass]
  public class MetaDataTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetMetaDataTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, Startup>());
      var resp = await srv.CreateClient().GetAsync("odata/v1/$metadata");
      var content = await resp.Content.ReadAsStringAsync();
      var doc = new XmlDocument();
      doc.LoadXml(content);
      Assert.IsTrue(doc.HasChildNodes);
      Assert.AreEqual(2, doc.ChildNodes.Count);
    }
  }
}
