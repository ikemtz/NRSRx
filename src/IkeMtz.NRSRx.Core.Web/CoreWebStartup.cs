using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using IkeMtz.NRSRx.Core.Web.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Jwt = System.IdentityModel.Tokens.Jwt;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Abstract base class for setting up a NRSRx application.
  /// </summary>
  public abstract class CoreWebStartup(IConfiguration configuration)
  {
    /// <summary>
    /// Gets the title of the microservice.
    /// </summary>
    public abstract string? ServiceTitle { get; }

    /// <summary>
    /// Gets the assembly of the startup class.
    /// </summary>
    public abstract Assembly? StartupAssembly { get; }

    /// <summary>
    /// Gets the route prefix for the Swagger UI.
    /// </summary>
    public virtual string SwaggerUiRoutePrefix { get; } = string.Empty;

    /// <summary>
    /// Gets the JWT name claim mapping.
    /// </summary>
    public virtual string JwtNameClaimMapping { get; } = JwtRegisteredClaimNames.Sub;

    /// <summary>
    /// Gets the JWT role claim mapping.
    /// </summary>
    public virtual string JwtRoleClaimMapping { get; } = "role";

    /// <summary>
    /// Gets a value indicating whether Swagger is disabled.
    /// </summary>
    public virtual bool DisableSwagger { get; }

    /// <summary>
    /// Gets a value indicating whether to include XML comments in Swagger documentation.
    /// </summary>
    public virtual bool IncludeXmlCommentsInSwaggerDocs { get; }

    /// <summary>
    /// Gets additional assembly XML document files.
    /// </summary>
    public virtual string[] AdditionalAssemblyXmlDocumentFiles { get; }

    /// <summary>
    /// Gets the OAuth scopes for Swagger.
    /// </summary>
    public virtual IEnumerable<OAuthScope> SwaggerScopes =>
      [
        OAuthScope.OpenId
      ];

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Sets up logging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="app">The application builder.</param>
    public virtual void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) { }

    /// <summary>
    /// Sets up application settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public virtual void SetupAppSettings(IServiceCollection services)
    {
      SetupCurrentUserProvider(services)
        .Configure<AppSettings>(Configuration)
        .AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value);
    }

    /// <summary>
    /// Sets up the current user provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public virtual IServiceCollection SetupCurrentUserProvider(IServiceCollection services)
    {
      return services
        .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
        .AddSingleton<ICurrentUserProvider, HttpUserProvider>();
    }

    /// <summary>
    /// Sets up the JWT authentication schema.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The authentication builder.</returns>
    public virtual AuthenticationBuilder SetupJwtAuthSchema(IServiceCollection services)
    {
      return services
          .AddAuthentication(options =>
          {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
          });
    }

    /// <summary>
    /// Sets up authentication.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public virtual void SetupAuthentication(AuthenticationBuilder builder)
    {
      Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
      JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
      _ = builder
          .AddJwtBearer(options =>
          {
            options.Authority = Configuration.GetValue<string>("IdentityProvider");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
              ValidateIssuer = true,
              ValidateIssuerSigningKey = true,
              NameClaimType = JwtNameClaimMapping,
              ValidAudiences = GetIdentityAudiences(),
              RoleClaimType = JwtRoleClaimMapping,
            };
          });
    }

    /// <summary>
    /// Gets the identity audiences.
    /// </summary>
    /// <param name="appSettings">The application settings.</param>
    /// <returns>The identity audiences.</returns>
    public virtual string[] GetIdentityAudiences(AppSettings? appSettings = null)
    {
      return (appSettings?.IdentityAudiences ?? Configuration.GetValue<string>("IdentityAudiences"))?.Split(',') ?? [];
    }

    /// <summary>
    /// Sets up the database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="dbConnectionString">The database connection string.</param>
    public virtual void SetupDatabase(IServiceCollection services, string dbConnectionString) { }

    /// <summary>
    /// Sets up MVC options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The MVC options.</param>
    public virtual void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
    }

    /// <summary>
    /// Sets up health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="healthChecks">The health checks builder.</param>
    public virtual void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder healthChecks)
    {

    }

    /// <summary>
    /// Sets up miscellaneous dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public virtual void SetupMiscDependencies(IServiceCollection services) { }

    /// <summary>
    /// Creates the default host builder.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class.</typeparam>
    /// <returns>The host builder.</returns>
    public static IHostBuilder CreateDefaultHostBuilder<TStartup>() where TStartup : CoreWebStartup
    {
      return Host.CreateDefaultBuilder()
       .ConfigureWebHostDefaults(webBuilder =>
       {
         _ = webBuilder.UseStartup<TStartup>();
       });
    }

    /// <summary>
    /// Sets up the common UI for Swagger.
    /// </summary>
    /// <param name="options">The Swagger UI options.</param>
    public virtual void SetupSwaggerCommonUi(SwaggerUIOptions options)
    {
      options.EnableDeepLinking();
      options.EnableFilter();
      options.DocumentTitle = $"{this.ServiceTitle} - Swagger UI";
      options.RoutePrefix = SwaggerUiRoutePrefix;
      options.HeadContent += "<meta name=\"robots\" content=\"none\" />";
      options.OAuthClientId(Configuration.GetValue<string>("SwaggerClientId"));
      options.OAuthClientSecret(Configuration.GetValue<string>("SwaggerClientSecret"));
      options.OAuthAppName(Configuration.GetValue<string>("SwaggerAppName"));
      options.OAuthScopeSeparator(" ");
      options.OAuthUsePkce();
    }

    /// <summary>
    /// Sets up Swagger generation options.
    /// </summary>
    /// <param name="options">The Swagger generation options.</param>
    /// <param name="xmlPath">The XML path for comments.</param>
    public virtual void SetupSwaggerGen(SwaggerGenOptions options, string? xmlPath = null)
    {
      // add a custom operation filter which sets default values
      options.OperationFilter<DefaultValueFilter>();
      options.OperationFilter<AuthorizeOperationFilter>();
      options.DocumentFilter<ReverseProxyDocumentFilter>();

      if (IncludeXmlCommentsInSwaggerDocs)
      {
        // Set the comments path for the Swagger JSON and UI.
        options.IncludeXmlComments(xmlPath ?? StartupAssembly.Location.Replace(".dll", ".xml", StringComparison.InvariantCultureIgnoreCase));
      }
      if (AdditionalAssemblyXmlDocumentFiles?.Length > 0)
      {
        AdditionalAssemblyXmlDocumentFiles.ToList().ForEach(f => options.IncludeXmlComments(f));
      }
    }

    private static OpenIdConfiguration OpenIdConfiguration;

    /// <summary>
    /// Gets the OpenID configuration.
    /// </summary>
    /// <param name="clientFactory">The HTTP client factory.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <returns>The OpenID configuration.</returns>
    public virtual OpenIdConfiguration? GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
    {
      if (OpenIdConfiguration != null)
      {
        return OpenIdConfiguration;
      }
      var resp = clientFactory.CreateClient()
          .GetAsync($"{appSettings?.IdentityProvider}.well-known/openid-configuration").Result;
      resp.EnsureSuccessStatusCode();
      var content = resp.Content.ReadAsStringAsync().Result;
      return OpenIdConfiguration = JsonConvert.DeserializeObject<OpenIdConfiguration>(content);
    }

    /// <summary>
    /// Gets the build number.
    /// </summary>
    /// <returns>The build number.</returns>
    public string GetBuildNumber()
    {
      return StartupAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ??
        StartupAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ??
        StartupAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
        "unknown";
    }
  }
}
