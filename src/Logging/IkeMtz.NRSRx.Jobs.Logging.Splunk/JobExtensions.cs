using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IkeMtz.NRSRx.Core.Jobs;

/// <summary>
/// Extension methods to setup logging on NRSRx framework
/// </summary>
public static class JobExtensions
{
  /// <summary>
  /// Sets up the asp.net core application to log to a Splunk instance using a Splunk Http Event Collector.
  /// Refernce: https://docs.splunk.com/Documentation/SplunkCloud/8.2.2106/Data/UsetheHTTPEventCollector
  /// Note: The following configuration variables are required:
  /// SPLUNK_HOST => The url of the splunk collector endpoint (ie: http(s)://{Splunk Host}/services/collector)
  /// SPLUNK_TOKEN => Security token 
  /// </summary>
  /// <param name="job"></param>
  /// <param name="services"></param>
  public static ILogger SetupSplunk(this IJob job, IServiceCollection services)
  {
    var splunkHost = job.Configuration.GetValue<string>("SPLUNK_HOST");
    var splunkToken = job.Configuration.GetValue<string>("SPLUNK_TOKEN");
    var splunkDisableSslValidation = job.Configuration.GetValue("SPLUNK_DISABLE_SSL_VALIDATION", false);
    var config = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .Enrich.WithMachineName()
          .WriteTo.Console();
    if (!string.IsNullOrWhiteSpace(splunkHost))
    {
      config = config.WriteTo.EventCollector(splunkHost, splunkToken,
          messageHandler: splunkDisableSslValidation ? new HttpClientHandler { ServerCertificateCustomValidationCallback = (x, y, z, a) => true } : null);
    }
    Log.Logger = config.CreateLogger();
    _ = services.AddLogging(x => x.AddSerilog());
    return Log.Logger;
  }
}

