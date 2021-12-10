using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Configures the Swagger generation options.
  /// </summary>
  /// <remarks>This allows API versioning to define a Swagger document per API version after the
  /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
  public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
  {
    private readonly IApiVersionDescriptionProvider provider;
    private readonly IODataVersionProvider odataVersionProvider;
    private readonly CoreWebStartup startup;
    private readonly string apiTitle;
    private readonly string buildNumber;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppSettings appSettings;

    public ConfigureSwaggerOptions(
      IServiceProvider serviceProvider,
      IConfiguration configuration,
      CoreWebStartup startup)
    {
      this.startup = startup;
      if (startup == null)
      {
        throw new ArgumentNullException(nameof(startup));
      }
      provider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
      odataVersionProvider = serviceProvider.GetService<IODataVersionProvider>();
      apiTitle = startup.MicroServiceTitle;
      buildNumber = startup.GetBuildNumber();

      this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
      this.appSettings = configuration.Get<AppSettings>();
    }

    public void Configure(SwaggerGenOptions options)
    {
      // add a swagger document for each discovered API version
      // note: you might choose to skip or document deprecated API versions differently
      if (provider != null)
      {
        foreach (var versionDescription in provider.ApiVersionDescriptions)
        {
          options.SwaggerDoc(versionDescription.GroupName, CreateInfoForApiVersion(versionDescription));
        }
      }
      if (odataVersionProvider != null)
      {
        foreach (var versionDescription in odataVersionProvider.GetODataVersions())
        {
          options.SwaggerDoc(versionDescription.GroupName, CreateInfoForApiVersion(versionDescription));
        }
      }
      var audiences = this.startup.GetIdentityAudiences(this.appSettings);
      if (audiences.Any() && !string.IsNullOrWhiteSpace(this.appSettings.IdentityProvider))
      {
        var discoveryDocument = startup.GetOpenIdConfiguration(this.httpClientFactory, appSettings);

        options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.OAuth2,
          In = ParameterLocation.Header,
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Flows = new OpenApiOAuthFlows
          {
            AuthorizationCode = new OpenApiOAuthFlow
            {
              AuthorizationUrl = discoveryDocument.GetAuthorizationEndpointUri(appSettings),
              TokenUrl = discoveryDocument.GetTokenEndpointUri(),
              Scopes = GetSwaggerScopeDictionary(startup.SwaggerScopes),
            }
          }
        });
      }
    }

    public static Dictionary<string, string> GetSwaggerScopeDictionary(IEnumerable<OAuthScope> swaggerScopes)
    {
      var swaggerScopeDictionary = new Dictionary<string, string>();
      swaggerScopes.ToList().ForEach(x =>
          swaggerScopeDictionary.TryAdd(x.Name, x.Description));
      return swaggerScopeDictionary;
    }

    public OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
      var info = new OpenApiInfo()
      {
        Title = apiTitle,
        Version = description?.ApiVersion.ToString(),
        Description = $"<div style='color:gray;font-weight:bold'>Build #: <span style='font-weight:bolder'>{this.buildNumber}</span></div>"
      };

      if ((description?.IsDeprecated).GetValueOrDefault())
      {
        info.Description += " This API version has been deprecated.";
      }
      return info;
    }
  }
}
