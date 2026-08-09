using System.Reflection;
using IkeMtz.NRSRx.Core.SignalR;
using IkeMtz.Samples.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.Samples.SignalR
{
  public class Startup(IConfiguration configuration) : CoreSignalrStartup(configuration)
  {
    public override Assembly StartupAssembly => typeof(Startup).Assembly;

    public override void MapHubs(IEndpointRouteBuilder endpoints)
    {
      _ = endpoints.MapHub<NotificationHub>("/notificationHub");
    }
  }
}
