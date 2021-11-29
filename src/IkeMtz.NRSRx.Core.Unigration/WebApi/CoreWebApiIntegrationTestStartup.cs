using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
  public class CoreWebApiIntegrationTestStartup<TStartup> : CoreWebApiTestStartup<TStartup>
        where TStartup : CoreWebApiStartup
  {
    
    public CoreWebApiIntegrationTestStartup(TStartup startup) : base(startup)
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
    public override void SetupPublishers(IServiceCollection services)
    {
      Startup.SetupPublishers(services);
    }
  }
}
