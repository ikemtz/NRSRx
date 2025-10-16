using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.SystemConsole.Themes;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Extension methods to setup Serilog logging in the NRSRx framework.
  /// </summary>
  public static class SeriLogExtensions
  {
    /// <summary>
    /// Configures the host builder to use Serilog for logging.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <returns>The configured host builder.</returns>
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
      return hostBuilder.UseSerilog();
    }

    /// <summary>
    /// Gets or sets the global Serilog logger instance.
    /// </summary>
    public static ILogger? Logger { get; set; }

    /// <summary>
    /// Sets up console logging using Serilog sinks.
    /// </summary>
    /// <param name="startup">The CoreWebStartup instance.</param>
    /// <param name="app">The application builder.</param>
    /// <param name="minimumLogLevelConfig">Callback to configure the minimum log level (default: Information).</param>
    /// <returns>The configured logger.</returns>
    public static ILogger SetupConsoleLogging(this CoreWebStartup startup, IApplicationBuilder? app, Func<LoggerMinimumLevelConfiguration, LoggerConfiguration>? minimumLogLevelConfig = null)
    {
      minimumLogLevelConfig ??= x => x.Information();
      _ = app?.UseSerilog();
      return GetLogger(() => minimumLogLevelConfig(new LoggerConfiguration().MinimumLevel)
           .Enrich.FromLogContext()
           .WriteTo.Console(theme: AnsiConsoleTheme.Code)
           .CreateLogger());
    }

    /// <summary>
    /// Gets or creates the logger instance.
    /// </summary>
    /// <param name="loggerFactory">The factory function to create the logger.</param>
    /// <returns>The logger instance.</returns>
    internal static ILogger GetLogger(Func<ILogger> loggerFactory)
    {
      if (Logger != null)
      {
        return Logger;
      }
      return Logger = Log.Logger = loggerFactory();
    }

    /// <summary>
    /// Configures the application to use Serilog request logging.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder? UseSerilog(this IApplicationBuilder app)
    {
      return app?.UseSerilogRequestLogging(options =>
      {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
              diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name ?? "Anonymous");
              diagnosticContext.Set("RemoteIpAddress", httpContext.Connection?.RemoteIpAddress?.ToString() ?? "255.255.255.255");
            };
      });
    }
  }
}
