using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Extension methods to setup logging on NRSRx framework
  /// </summary>
  public static class StartupExtensions
  {
    /// <summary>
    /// Sets up the asp.net core application to log to a Splunk instance using a Splunk Http Event Collector.
    /// Refernce: https://docs.splunk.com/Documentation/SplunkCloud/8.2.2106/Data/UsetheHTTPEventCollector
    /// Note: The following configuration variables are required:
    /// SPLUNK_HOST => The url of the splunk collector endpoint (ie: http(s)://{Splunk Host}/services/collector)
    /// SPLUNK_TOKEN => Security token 
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="app"></param>
    public static ILogger SetupSplunk(this CoreWebStartup startup, IApplicationBuilder? app)
    {
      _ = app?.UseSerilog();

      var splunkHost = startup.Configuration.GetValue<string>("SPLUNK_HOST");
      var splunkToken = startup.Configuration.GetValue<string>("SPLUNK_TOKEN");
      var splunkDisableSslValidation = startup.Configuration.GetValue("SPLUNK_DISABLE_SSL_VALIDATION", false);
      var splunkUriPath = startup.Configuration.GetValue("SPLUNK_URI_PATH", "services/collector/event");
      var splunkIndex = startup.Configuration.GetValue("SPLUNK_INDEX", "");
      var splunkSourceType = startup.Configuration.GetValue("SPLUNK_SOURCE_TYPE", "");
      var host = startup.Configuration.GetValue("ENVIRONMENT_NAME", "");
      return SeriLogExtensions.GetLogger(() => new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.EventCollector(
          splunkHost,
          splunkToken,
          uriPath: splunkUriPath,
          index: splunkIndex,
          sourceType: splunkSourceType,
          host: host,
          messageHandler: splunkDisableSslValidation ? new HttpClientHandler
          {
            ServerCertificateCustomValidationCallback = (reqMsg, cert, certChain, sslPolicyErr) => true
          } : null)
        .CreateLogger());
    }
  }
}

