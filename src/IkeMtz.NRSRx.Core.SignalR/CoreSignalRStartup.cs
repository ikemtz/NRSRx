using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.SignalR
{
  /// <summary>
  /// Abstract base class for setting up a SignalR application.
  /// </summary>
  /// <param name="configuration">The configuration.</param>
  public abstract class CoreSignalrStartup(IConfiguration configuration) : CoreWebStartup(configuration)
  {
    /// <summary>
    /// Gets the title of the microservice.
    /// </summary>
    public override string? ServiceTitle => null;

    /// <summary>
    /// Gets the assembly of the startup class.
    /// </summary>
    public override Assembly? StartupAssembly => null;

    /// <summary>
    /// Adds services to the container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
      SetupLogging(services);
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupUserIdProvider(services);
      SetupMiscDependencies(services);
      _ = services.AddSignalR();
      var healthCheckBuilder = services.AddHealthChecks();
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupHealthChecks(services, healthCheckBuilder);
    }

    /// <summary>
    /// Sets up the user ID provider for SignalR communications.
    /// </summary>
    /// <param name="services">The service collection to add the user ID provider to.</param>
    public virtual void SetupUserIdProvider(IServiceCollection services)
    {
      services.AddSingleton<IUserIdProvider, UserIdProvider>();
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web host environment.</param>
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      _ = app
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseEndpoints(endpoints =>
        {
          _ = endpoints.MapHealthChecks("/healthz");
          MapHubs(endpoints);
        });
    }

    /// <summary>
    /// Sets up authentication.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
      JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
      _ = builder
          .AddJwtBearer(options =>
          {
            options.Authority = Configuration.GetValue<string>("IdentityProvider");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
              ValidateIssuer = true,
              ValidateIssuerSigningKey = true,
              NameClaimType = JwtNameClaimMapping,
              ValidAudiences = GetIdentityAudiences(),
              RoleClaimType = JwtRoleClaimMapping,
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

    /// <summary>
    /// Maps SignalR hubs to endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public abstract void MapHubs(IEndpointRouteBuilder endpoints);
  }
}
