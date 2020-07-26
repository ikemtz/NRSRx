using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    private readonly CoreWebStartup startup;
    private readonly string apiTitle;
    private readonly string buildNumber;
    private readonly IHttpClientFactory httpClientFactory;
    private AppSettings appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
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
      this.provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
      this.apiTitle = startup.MicroServiceTitle;
      this.buildNumber = startup.StartupAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ??
      startup.StartupAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ??
      startup.StartupAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
      "unknown";

      this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
      this.appSettings = configuration.Get<AppSettings>();
    }


    /// <inheritdoc />
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    public void Configure(SwaggerGenOptions options)
    {
      var discoveryDocument = startup.GetOpenIdConfiguration(this.httpClientFactory, appSettings);

      // add a swagger document for each discovered API version
      // note: you might choose to skip or document deprecated API versions differently
      foreach (var description in provider.ApiVersionDescriptions)
      {
        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
      }
      var audiences = this.startup.GetIdentityAudiences(this.appSettings);
      if (audiences.Any() && !string.IsNullOrWhiteSpace(this.appSettings.IdentityProvider))
      {
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
              TokenUrl =  discoveryDocument.GetTokenEndpointUri(),
              Scopes = this.startup.SwaggerScopes,
            }
          }
        });
      }
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
