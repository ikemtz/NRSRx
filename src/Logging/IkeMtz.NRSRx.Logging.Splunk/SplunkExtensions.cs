using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace IkeMtz.NRSRx.Logging.Splunk
{
  public static partial class SplunkExtensions
  {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static HttpClientHandler _clientHandler;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static HttpClientHandler ClientHandler
    {
      get
      {
        _clientHandler ??= new HttpClientHandler { ServerCertificateCustomValidationCallback = (x, y, z, a) => true }; //NOSONAR
        return _clientHandler;
      }
    }
    public static ILogger ConfigureSplunkLogger(IConfiguration configuration)
    {
      var splunkHost = configuration.GetValue<string>("SPLUNK_HOST");
      var splunkToken = configuration.GetValue<string>("SPLUNK_TOKEN");
      var splunkDisableSslValidation = configuration.GetValue("SPLUNK_DISABLE_SSL_VALIDATION", false);
      var splunkUriPath = configuration.GetValue("SPLUNK_URI_PATH", "services/collector/event");
      var splunkIndex = configuration.GetValue("SPLUNK_INDEX", "");
      var splunkSourceType = configuration.GetValue("SPLUNK_SOURCE_TYPE", "");
      var host = configuration.GetValue("ENVIRONMENT_NAME", "");
      var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console();
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
