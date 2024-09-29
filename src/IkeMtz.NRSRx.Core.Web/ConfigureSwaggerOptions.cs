using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.NRSRx.Core.Web.Swagger;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="startup">The CoreWebStartup instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when the startup parameter is null.</exception>
    public ConfigureSwaggerOptions(
      IServiceProvider serviceProvider,
      IConfiguration configuration,
      CoreWebStartup startup)
    {
      this.startup = startup ?? throw new ArgumentNullException(nameof(startup));
      provider = serviceProvider.GetService<IApiVersionDescriptionProvider>();
      odataVersionProvider = serviceProvider.GetService<IODataVersionProvider>();
      apiTitle = startup.ServiceTitle;
      buildNumber = startup.GetBuildNumber();
      httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
      appSettings = configuration.Get<AppSettings>();
    }

    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <param name="options">The Swagger generation options.</param>
    public void Configure(SwaggerGenOptions options)
    {
      // Add a Swagger document for each discovered API version
      if (provider != null)
      {
        foreach (var versionDescription in provider.ApiVersionDescriptions)
        {
          options.SwaggerDoc(versionDescription.GroupName, CreateInfoForApiVersion(versionDescription));
          options.SchemaFilter<EnumSchemaFilter>();
        }
      }
      if (odataVersionProvider != null)
      {
        foreach (var versionDescription in odataVersionProvider.GetODataVersions())
        {
          options.SwaggerDoc(versionDescription.GroupName, CreateInfoForApiVersion(versionDescription));
          options.SchemaFilter<EnumSchemaFilter>();
        }
      }
      var audiences = startup.GetIdentityAudiences(appSettings);
      if (audiences.Any() && !string.IsNullOrWhiteSpace(appSettings.IdentityProvider))
      {
        var discoveryDocument = startup.GetOpenIdConfiguration(httpClientFactory, appSettings);

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

    /// <summary>
    /// Creates a dictionary of Swagger scopes.
    /// </summary>
    /// <param name="swaggerScopes">The collection of OAuth scopes.</param>
    /// <returns>A dictionary of Swagger scopes.</returns>
    public static Dictionary<string, string> GetSwaggerScopeDictionary(IEnumerable<OAuthScope> swaggerScopes)
    {
      var swaggerScopeDictionary = new Dictionary<string, string>();
      swaggerScopes.ToList().ForEach(x =>
          swaggerScopeDictionary.TryAdd(x.Name, x.Description));
      return swaggerScopeDictionary;
    }

    /// <summary>
    /// Creates the OpenAPI information for a specific API version.
    /// </summary>
    /// <param name="description">The API version description.</param>
    /// <returns>The OpenAPI information.</returns>
    public OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
      var info = new OpenApiInfo
      {
        Title = apiTitle,
        Version = description?.ApiVersion.ToString(),
        Description = $"<div style='color:gray;font-weight:bold'>Build #: <span style='font-weight:bolder'>{buildNumber}</span></div>"
      };

      if ((description?.IsDeprecated).GetValueOrDefault())
      {
        info.Description += " This API version has been deprecated.";
      }
      return info;
    }
  }
}
