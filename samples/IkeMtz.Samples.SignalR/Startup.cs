using System.Reflection;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.SignalR;
using IkeMtz.Samples.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.SignalR
{
  public class Startup : CoreSignalrStartup
  {
    public override Assembly StartupAssembly => typeof(Startup).Assembly;
    public Startup(IConfiguration configuration) : base(configuration) { }

    public override void SetupLogging(IServiceCollection services)
    {
      this.SetupApplicationInsights(services);
    }

    public override void MapHubs(IEndpointRouteBuilder endpoints)
    {
      _ = endpoints.MapHub<NotificationHub>("/notificationHub");
    }
  }
}
