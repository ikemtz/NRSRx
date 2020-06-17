using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
      _ = services.AddSignalR();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      _ = app
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization()
        .UseEndpoints(MapHubs);
    }

    public abstract void MapHubs(IEndpointRouteBuilder endpoints);
  }
}
