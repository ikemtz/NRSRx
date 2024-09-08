using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.SystemConsole.Themes;

namespace IkeMtz.NRSRx.Core.Web
{
  public static class SeriLogExtensions
  {
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
      return hostBuilder.UseSerilog();
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal static ILogger Logger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Sets up Console Logging only, leverages SeriLog sinks
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="app"></param>
    /// <param name="minimumLogLevel">Use this callback to configure your preferred level of logging (default: Information)</param>
    public static ILogger SetupConsoleLogging(this CoreWebStartup startup, IApplicationBuilder? app, Func<LoggerMinimumLevelConfiguration, LoggerConfiguration>? minimumLogLevelConfig = null)
    {
      minimumLogLevelConfig ??= X => X.Information();
      _ = (app?.UseSerilog());
      return GetLogger(() => minimumLogLevelConfig(new LoggerConfiguration().MinimumLevel)
           .Enrich.FromLogContext()
           .WriteTo.Console(theme: AnsiConsoleTheme.Code)
           .CreateLogger());
    }

    internal static ILogger GetLogger(Func<ILogger> loggerFactory)
    {
      if (Logger != null)
      {
        return Logger;
      }
      return Logger = Log.Logger = loggerFactory();
    }

    public static IApplicationBuilder? UseSerilog(this IApplicationBuilder app)
    {
      return app?.UseSerilogRequestLogging(options =>
      {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
          diagnosticContext.Set("UserName", httpContext.User?.Identity?.Name);
          diagnosticContext.Set("RemoteIpAddress", httpContext.Connection?.RemoteIpAddress?.ToString());
        };
      });
    }
  }
}
