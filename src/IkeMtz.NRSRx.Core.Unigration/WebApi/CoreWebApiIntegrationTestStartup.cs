using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
    public class CoreWebApiIntegrationTestStartup<Startup> : CoreWebApiTestStartup<Startup>
        where Startup : CoreWebApiStartup
    {
        public CoreWebApiIntegrationTestStartup(Startup startup) : base(startup)
        {
        }

        public override void SetupAuthentication(AuthenticationBuilder builder)
        {
            startup.SetupAuthentication(builder);
        }

        public override void SetupDatabase(IServiceCollection services, string connectionString)
        {
            startup.SetupDatabase(services, connectionString);
        }
        public override void SetupPublishers(IServiceCollection services)
        {
            startup.SetupPublishers(services);
        }
    }
}
