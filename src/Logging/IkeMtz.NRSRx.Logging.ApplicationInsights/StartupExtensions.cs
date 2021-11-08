using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Extension methods to setup logging on NRSRx framework
  /// </summary>
  public static class StartupExtensions
  {
    /// <summary>
    /// Sets up application to utlize Application Insights on Azure.
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="services"></param>
    public static void SetupApplicationInsights(this CoreWebStartup startup, IServiceCollection services)
    {
      _ = services?
          .AddApplicationInsightsTelemetry(
            new ApplicationInsightsServiceOptions()
            {

              InstrumentationKey = startup.Configuration.GetValue<string>("InstrumentationKey"),
              ApplicationVersion = startup.GetBuildNumber(),
              EnableDiagnosticsTelemetryModule = true,
            });
    }

    /// <summary>
    /// Sets up application to utlize Application Insights on Azure in development mode.
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="services"></param>
    public static void SetupDevelopmentApplicationInsights(this CoreWebStartup startup, IServiceCollection services)
    {
      _ = services?
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
