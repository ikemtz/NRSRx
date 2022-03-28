using System;
using System.Net.Http;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class OpenIdConfigurationTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void GetOpenIdConfigurationTest()
    {
      var appSettings = new AppSettings
      {
        IdentityProvider = "https://accounts.google.com/",
        IdentityAudiences = "x,y,z"
      };
      var startup = new StartUp_AppInsights(null);
      var result = startup.GetOpenIdConfiguration(new TestHttpClientFactory(), appSettings);
      Assert.AreEqual($"{appSettings.IdentityProvider}o/oauth2/v2/auth", result.AuthorizeEndpoint);
      Assert.AreEqual(new Uri($"{ appSettings.IdentityProvider}o/oauth2/v2/auth?audience=x"), result.GetAuthorizationEndpointUri(appSettings));
      Assert.AreEqual(new Uri("https://oauth2.googleapis.com/token"), result.GetTokenEndpointUri());
    }
  }

  public class TestHttpClientFactory : IHttpClientFactory
  {
    public HttpClient CreateClient(string name)
    {
      return new HttpClient();
    }
  }
}
