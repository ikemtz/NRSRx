using IkeMtz.NRSRx.Core.Web;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace IkeMtz.NRSRx.Core
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
    public static ILogger SetupSplunk(this CoreWebStartup startup)
    {
      var splunkHost = startup.Configuration.GetValue<string>("SPLUNK_HOST");
      var splunkToken = startup.Configuration.GetValue<string>("SPLUNK_TOKEN");
      return Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.EventCollector(splunkHost, splunkToken)
          .CreateLogger();
    }
    /// <summary>
    /// Sets up Console Logging only, leverages SeriLog sinks
    /// </summary>
    public static ILogger SetupConsoleLogging(this CoreWebStartup startup)
    {
      if (startup is null)
      {
        throw new System.ArgumentNullException(nameof(startup));
      }

      return Log.Logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .CreateLogger();
    }
  }
}
