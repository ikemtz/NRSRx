using IkeMtz.NRSRx.Logging.Splunk;
using Microsoft.AspNetCore.Builder;
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
      return SeriLogExtensions.GetLogger(() => SplunkExtensions.ConfigureSplunkLogger(startup.Configuration));
    }
  }
}

