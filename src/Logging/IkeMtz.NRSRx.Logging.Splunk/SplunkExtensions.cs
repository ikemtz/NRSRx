using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.SystemConsole.Themes;

namespace IkeMtz.NRSRx.Logging.Splunk
{
  /// <summary>
  /// Extension methods to setup logging to Splunk in the NRSRx framework.
  /// </summary>
  public static partial class SplunkExtensions
  {
    private static HttpClientHandler _clientHandler;

    /// <summary>
    /// Gets the HTTP client handler with SSL validation disabled.
    /// </summary>
    public static HttpClientHandler ClientHandler
    {
      get
      {
        _clientHandler ??= new HttpClientHandler { ServerCertificateCustomValidationCallback = (x, y, z, a) => true }; //NOSONAR
        return _clientHandler;
      }
    }

    /// <summary>
    /// Configures the logger to log to a Splunk instance.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="minimumLogLevelConfig">Callback to configure the minimum log level (default: Information).</param>
    /// <returns>The configured logger.</returns>
    /// <remarks>
    /// The following configuration variables are required:
    /// - SPLUNK_HOST: The URL of the Splunk HTTP Event Collector (HEC) endpoint.
    /// - SPLUNK_TOKEN: The authentication token for the Splunk HEC.
    /// 
    /// The following configuration values are optional:
    /// - SPLUNK_DISABLE_SSL_VALIDATION: Set to "true" to disable SSL validation (default: false).
    /// - SPLUNK_URI_PATH: The URI path for the Splunk HEC (default: "services/collector/event").
    /// - SPLUNK_INDEX: The index to send events to (default: empty).
    /// - SPLUNK_SOURCE_TYPE: The source type for the events (default: empty).
    /// - ENVIRONMENT_NAME: The environment name to include in the logs (default: empty).
    /// </remarks>
    public static ILogger ConfigureSplunkLogger(IConfiguration configuration, Func<LoggerMinimumLevelConfiguration, LoggerConfiguration>? minimumLogLevelConfig = null)
    {
      minimumLogLevelConfig ??= x => x.Information();
      var splunkHost = configuration.GetValue<string>("SPLUNK_HOST");
      var splunkToken = configuration.GetValue<string>("SPLUNK_TOKEN");
      var splunkDisableSslValidation = configuration.GetValue("SPLUNK_DISABLE_SSL_VALIDATION", false);
      var splunkUriPath = configuration.GetValue("SPLUNK_URI_PATH", "services/collector/event");
      var splunkIndex = configuration.GetValue("SPLUNK_INDEX", "");
      var splunkSourceType = configuration.GetValue("SPLUNK_SOURCE_TYPE", "");
      var host = configuration.GetValue("ENVIRONMENT_NAME", "");
      var config = minimumLogLevelConfig(new LoggerConfiguration().MinimumLevel)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code);
      if (!string.IsNullOrWhiteSpace(splunkHost))
      {
        config = config.WriteTo.EventCollector(splunkHost,
          splunkToken,
          uriPath: splunkUriPath,
          index: splunkIndex,
          sourceType: splunkSourceType,
          host: host,
          messageHandler: splunkDisableSslValidation ? ClientHandler : null);
      }
      return Log.Logger = config.CreateLogger();
    }
  }
}
