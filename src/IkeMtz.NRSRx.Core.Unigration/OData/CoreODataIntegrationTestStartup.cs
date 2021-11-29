using System;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreODataIntegrationTestStartup<TStartup>
        : CoreODataTestStartup<TStartup>
        where TStartup : CoreODataStartup
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
      Startup.SetupDatabase(services, !string.IsNullOrWhiteSpace(dbConnectionString) ?
        dbConnectionString : Environment.GetEnvironmentVariable("DbConnectionString"));
    }
  }
}
