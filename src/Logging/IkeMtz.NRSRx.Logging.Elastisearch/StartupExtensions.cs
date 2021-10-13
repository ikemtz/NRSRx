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
    /// Sets up the asp.net core application to log to a Elastisearch instance.
    /// Refernce: https://github.com/serilog/serilog-sinks-elasticsearch/wiki/basic-setup
    /// Note: The following configuration variables are required:
    /// ELASTISEARCH_HOST => The url of the Elastisearch endpoint (ie: http(s)://{Elastisearch Host}/9200)
    /// </summary>
    /// <param name="startup"></param>
    public static ILogger SetupElastisearch(this CoreWebStartup startup)
    {
      var host = startup.Configuration.GetValue<string>("ELASTISEARCH_HOST", "http://localhost:9200");
      return Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.Elasticsearch(host)
          .CreateLogger();
    }
    /// <summary>
    /// Sets up Console Logging only, leverages SeriLog sinks
    /// </summary>
    /// <param name="startup"></param>
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
