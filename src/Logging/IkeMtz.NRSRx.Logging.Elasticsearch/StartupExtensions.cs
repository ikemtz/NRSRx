using System;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Extension methods to setup logging on NRSRx framework
  /// </summary>
  public static class StartupExtensions
  {
    /// <summary>
    /// Sets up the asp.net core application to log to a Elasticsearch instance.
    /// Refernce: https://github.com/serilog/serilog-sinks-elasticsearch/wiki/basic-setup
    /// Note: The following configuration variables are required:
    /// ELASTICSEARCH_HOST => The url of the Elasticsearch endpoint (ie: http(s)://{Elasticsearch Host}/9200)
    /// Note: The following configuration values should be set for Elasticsearch basic authentication
    /// ELASTICSEARCH_USERNAME
    /// ELASTICSEARCH_PASSWORD
    /// Note: The following configuration values should be set for Elasticsearch api key authentication
    /// ELASTICSEARCH_USERNAME => This should be the "id" of your token
    /// ELASTICSEARCH_APIKEY => This should be the "api_key" of your token
    /// Note: If your Elasticsearch instance is using an invalid SSL cert
    /// ELASTICSEARCH_DISABLE_SSL_VALIDATION => set this value to "true"
    /// Note: This library only has support for v6.x and v7.x versions of Elasticsearch, to use 6.x provide the following:
    /// ELASTICSEARCH_VERSION => set this value to 6.x
    /// </summary>
    /// <param name="startup"></param>
    /// <param name="app"></param>
    public static ILogger SetupElasticsearch(this CoreWebStartup startup, IApplicationBuilder? app)
    {
      _ = (app?.UseSerilog());

      return SeriLogExtensions.GetLogger(() =>
      {
        var environment = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME");

        var host = startup.Configuration.GetValue("ELASTICSEARCH_HOST", "http://localhost:9200");
        var username = startup.Configuration.GetValue<string>("ELASTICSEARCH_USERNAME");
        var password = startup.Configuration.GetValue<string>("ELASTICSEARCH_PASSWORD");
        var apiKey = startup.Configuration.GetValue<string>("ELASTICSEARCH_APIKEY");
        var elastiOptions = new ElasticsearchSinkOptions(new Uri(host))
        {
          IndexFormat = $"{startup.StartupAssembly?.GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yy-MM}",
          AutoRegisterTemplate = true,
          AutoRegisterTemplateVersion =
          startup.Configuration.GetValue("ELASTICSEARCH_VERSION", "7x").StartsWith("6") ? AutoRegisterTemplateVersion.ESv6
          : AutoRegisterTemplateVersion.ESv7,
        };
        var modifyConfigSettings = new Func<Func<ConnectionConfiguration>, ConnectionConfiguration>((authFunc) =>
        {
          var config = authFunc();
          if (startup.Configuration.GetValue<bool>("ELASTICSEARCH_DISABLE_SSL_VALIDATION"))
          {
            _ = config.ServerCertificateValidationCallback((obj, cert, chain, policyErrors) => true);
          }
          return config;
        });
        if (!string.IsNullOrWhiteSpace(password))
        {
          elastiOptions.ModifyConnectionSettings = config => modifyConfigSettings(() => config.BasicAuthentication(username, password));
        }
        else if (!string.IsNullOrWhiteSpace(apiKey))
        {
          elastiOptions.ModifyConnectionSettings = config => modifyConfigSettings(() => config.ApiKeyAuthentication(username, apiKey));
        }
        return new LoggerConfiguration() { }
          .MinimumLevel.Information()
          .Enrich.FromLogContext()
          .Enrich.WithMachineName()
          .WriteTo.Console(theme: AnsiConsoleTheme.Code)
          .WriteTo.Elasticsearch(elastiOptions)
          .Enrich.WithProperty("Environment", environment)
          .CreateLogger();
      });
    }
  }
}
