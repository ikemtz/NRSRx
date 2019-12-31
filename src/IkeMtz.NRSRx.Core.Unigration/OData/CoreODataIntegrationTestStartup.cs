using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreODataIntegrationTestStartup<Startup, ModelConfiguration>
        : CoreODataTestStartup<Startup, ModelConfiguration>
        where Startup : CoreODataStartup
        where ModelConfiguration : IModelConfiguration, new()
  {
    public CoreODataIntegrationTestStartup(Startup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      _startup.SetupAuthentication(builder);
    }

    public override void SetupDatabase(IServiceCollection services, string connectionString)
    {
      _startup.SetupDatabase(services, connectionString);
    }
  }
}
