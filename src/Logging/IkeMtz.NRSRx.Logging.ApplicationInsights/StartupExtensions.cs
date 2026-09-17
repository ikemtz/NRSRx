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
    /// Sets up application to utilize Application Insights on Azure.
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="services"></param>
    /// <param name="options"></param>
    public static void SetupApplicationInsights(this CoreWebStartup startup, IServiceCollection? services, ApplicationInsightsServiceOptions? options = null)
    {
      var appInsightsConnectionString = startup.Configuration.GetValue<string>("InstrumentationConnectionString");
      if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
      {
        _ = services?
          .AddApplicationInsightsTelemetry(
           options ?? new ApplicationInsightsServiceOptions()
           {
             ConnectionString = appInsightsConnectionString,
             ApplicationVersion = startup.GetBuildNumber(),
             EnableDependencyTrackingTelemetryModule = true,
             EnableRequestTrackingTelemetryModule = true,
             EnablePerformanceCounterCollectionModule = true,
           });
      }
    }

    /// <summary>
    /// Sets up application to utilize Application Insights on Azure in development mode.
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="services"></param>
    public static void SetupDevelopmentApplicationInsights(this CoreWebStartup startup, IServiceCollection? services)
    {
      var appInsightsConnectionString = startup.Configuration.GetValue<string>("InstrumentationConnectionString");

      SetupApplicationInsights(startup, services, new ApplicationInsightsServiceOptions
      {
        ConnectionString = appInsightsConnectionString,
        ApplicationVersion = startup.GetBuildNumber(),
        //EnableDependencyTrackingTelemetryModule = true,
        //EnablePerformanceCounterCollectionModule = true,
        //EnableRequestTrackingTelemetryModule = true,
      });
    }
  }
}
