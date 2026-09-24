using System.Collections.Generic;
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
  [TestCategory(TestCategories.Unigration)]
  public class SwaggerPageTests : BaseUnigrationTests
  {
    [TestMethod]
    public async Task GetSwaggerPageTest()
    {
      var myConfiguration = new Dictionary<string, string?>
      {
        //{ReverseProxyDocumentFilter.SwaggerReverseProxyBasePath, "/my-api"},
      };
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureAppConfiguration((builderContext, configurationBuilder) =>
          configurationBuilder.AddInMemoryCollection(myConfiguration)
        ));
      var htmlPage = await OpenApiUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(htmlPage);
      var jsonDoc = await OpenApiUnitTests.TestJsonDocAsync(srv, new Startup(null));
      Assert.IsNotNull(jsonDoc);
    }
    [TestMethod]    
    public void ValidateODataEnvelopName()
    {
      var result = new BaseUnigrationTests().GetODataEnvelopeName<School>();
      Assert.AreEqual("ODataEnvelopeOfSchoolAndGuid", result);
    }
  }
}
