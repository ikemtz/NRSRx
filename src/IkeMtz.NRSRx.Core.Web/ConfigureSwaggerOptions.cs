using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

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
    private readonly string apiTitle;
    private readonly string buildNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, CoreWebStartup startup)
    {
      if (startup == null)
      {
        throw new ArgumentNullException(nameof(startup));
      }
      this.provider = provider;
      this.apiTitle = startup.MicroServiceTitle;
      this.buildNumber = startup.StartupAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ??
      startup.StartupAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ??
      startup.StartupAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
      "unknown";
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
      // add a swagger document for each discovered API version
      // note: you might choose to skip or document deprecated API versions differently
      foreach (var description in provider.ApiVersionDescriptions)
      {
        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
      }
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
      var info = new OpenApiInfo()
      {
        Title = apiTitle,
        Version = description.ApiVersion.ToString(),
        Description = $"<div style='color:gray;font-weight:bold'>Build #: <span style='font-weight:bolder'>{this.buildNumber}</span></div>"
      };

      if (description.IsDeprecated)
      {
        info.Description += " This API version has been deprecated.";
      }

      return info;
    }
  }
}
