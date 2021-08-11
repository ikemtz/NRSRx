using IkeMtz.NRSRx.Core.Web;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core
{
  public static class StartupExtensions
  {
    public static void SetupApplicationInsights(this CoreWebStartup startup, IServiceCollection services)
    {
      _ = services
          .AddApplicationInsightsTelemetry(
            new ApplicationInsightsServiceOptions()
            {
              InstrumentationKey = startup.Configuration.GetValue<string>("InstrumentationKey"),
              ApplicationVersion = startup.GetBuildNumber(),
              EnableDiagnosticsTelemetryModule = true,
            });
    }
    public static void SetupDevelopmentApplicationInsights(this CoreWebStartup startup, IServiceCollection services)
    {
      _ = services
          .AddApplicationInsightsTelemetry(
            new ApplicationInsightsServiceOptions()
            {
              InstrumentationKey = startup.Configuration.GetValue<string>("InstrumentationKey"),
              ApplicationVersion = startup.GetBuildNumber(),
              EnableDiagnosticsTelemetryModule = true,
              DeveloperMode = true,
              EnableDebugLogger = true,
            });
    }
  }
}
