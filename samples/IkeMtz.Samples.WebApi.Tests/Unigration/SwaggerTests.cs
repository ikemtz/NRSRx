using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Swagger;
using IkeMtz.NRSRx.Core.Web.Swagger;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.WebApi.Tests.Unigration
{
  [TestClass]
  public class SwaggerTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSwaggerIndexPageTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationWebApiTestStartup>());
      var html = await SwaggerUnitTests.TestHtmlPageAsync(srv);
      Assert.IsNotNull(html);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetSwaggerJsonTest()
    {
      var myConfiguration = new Dictionary<string, string>
      {
        {ReverseProxyDocumentFilter.SwaggerReverseProxyBasePath, "/my-api"},
      };
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationWebApiTestStartup>()
        .ConfigureAppConfiguration((builderContext, configurationBuilder) =>
          configurationBuilder.AddInMemoryCollection(myConfiguration)
        ));
      var doc = await SwaggerUnitTests.TestJsonDocAsync(srv);
      _ = await SwaggerUnitTests.TestReverseProxyJsonDocAsync(srv, "/my-api");
      Assert.IsTrue(doc.Components.Schemas.ContainsKey(nameof(Course)));
      Assert.AreEqual($"{nameof(Samples)} WebApi Microservice", doc.Info.Title);
    }
  }
}
