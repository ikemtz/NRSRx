using System.Reflection;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.SignalR
{
  public abstract class CoreSignalrStartup : CoreWebStartup
  {
    public override string MicroServiceTitle => null;
    public override Assembly StartupAssembly => null;
    protected CoreSignalrStartup(IConfiguration configuration) : base(configuration)
    {
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
      SetupLogging(services);
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      _ = services.AddSignalR();
      var healthCheckBuilder = services.AddHealthChecks();
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupHealthChecks(services, healthCheckBuilder);
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      _ = app
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseEndpoints(endpoints =>
        {
          _ = endpoints.MapHealthChecks("/health");
          MapHubs(endpoints);
        });
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      _ = builder
          .AddJwtBearer(options =>
          {
            options.Authority = Configuration.GetValue<string>("IdentityProvider");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
              NameClaimType = JwtNameClaimMapping,
              ValidAudiences = GetIdentityAudiences(),
            };
            options.Events = new JwtBearerEvents()
            {
              OnMessageReceived = messageReceivedContext =>
              {
                if (messageReceivedContext.Request.Query.TryGetValue("access_token", out StringValues accessToken))
                {
                  messageReceivedContext.Token = accessToken;
                }
                return Task.CompletedTask;
              }
            };
          });
    }
    public abstract void MapHubs(IEndpointRouteBuilder endpoints);
  }
}
