using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.WebApi;
using IkeMtz.Samples.Events.Redis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.Events.Tests.Integration
{
  public class UnigrationEventsTestStartup
      : CoreWebApiIntegrationTestStartup<Startup>
  {
    public UnigrationEventsTestStartup(IConfiguration configuration)
        : base(new Startup(configuration))
    {
    }
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) =>
      Startup.SetupLogging(services);


    public override void SetupPublishers(IServiceCollection services)
    {
    }
  }
}
