using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreODataIntegrationTestStartup<TStartup, TModelConfiguration>
        : CoreODataTestStartup<TStartup, TModelConfiguration>
        where TStartup : CoreODataStartup
        where TModelConfiguration : IModelConfiguration, new()
  {
    public CoreODataIntegrationTestStartup(TStartup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      Startup.SetupAuthentication(builder);
    }

    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      Startup.SetupDatabase(services, dbConnectionString);
    }
  }
}
