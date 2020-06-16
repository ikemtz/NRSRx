using System;
using System.Reflection;
using IkeMtz.NRSRx.Core.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using IkeMtz.Samples.SignalR.Hubs;

namespace IkeMtz.Samples.SignalR
{
  public class Startup : CoreSignalrStartup
  {
    public Startup(IConfiguration configuration) : base(configuration) { }

    public override string MicroServiceTitle => "IkeMtz Sample SignalR Service";

    public override Assembly StartupAssembly => typeof(Startup).Assembly;

    public override void MapHubs(IEndpointRouteBuilder endpoints)
    {
      _ = endpoints.MapHub<NotificationHub>("/notificationHub");
    }
  }
}
