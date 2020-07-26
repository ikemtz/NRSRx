using System.Net.Http;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreODataUnigrationTestStartup<TStartup, TModelConfiguration>
        : CoreODataTestStartup<TStartup, TModelConfiguration>
        where TStartup : CoreODataStartup
        where TModelConfiguration : IModelConfiguration, new()
  {
    public CoreODataUnigrationTestStartup(TStartup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }

    public override OpenIdConfiguration GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
    {
      return new OpenIdConfiguration
      {
        AuthorizeEndpoint = "https://demo.identityserver.io/connect/authorize",
        TokenEndpoint = $"https://demo.identityserver.io/connect/token",
      };
    }
  }
}
