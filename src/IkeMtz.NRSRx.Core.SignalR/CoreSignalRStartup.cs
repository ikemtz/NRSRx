using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.SignalR
{
  public abstract class CoreSignalrStartup : CoreWebStartup
  {
    protected CoreSignalrStartup(IConfiguration configuration) : base(configuration)
    {
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
      SetupLogging(services);
      SetupAuthentication(SetupJwtAuthSchema(services));
      _ = services
      .AddSignalR();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        _ = app.UseDeveloperExceptionPage();
      }
      _ = app.UseRouting()
         .UseEndpoints(MapHubs);
    }

    public abstract void MapHubs(IEndpointRouteBuilder endpoints);
  }
}
