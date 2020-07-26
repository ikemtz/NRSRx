using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace IkeMtz.NRSRx.Core.Web
{
  public abstract class CoreWebStartup
  {
    public abstract string MicroServiceTitle { get; }
    public abstract Assembly StartupAssembly { get; }
    public virtual string JwtNameClaimMapping { get; } = JwtRegisteredClaimNames.Sub;
    public virtual Dictionary<string, string> SwaggerScopes =>
        new Dictionary<string, string>{
                        { "openid", "required" }
                };
    public IConfiguration Configuration { get; }
    protected CoreWebStartup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public virtual void SetupLogging(IServiceCollection services)
    {
      _ = services
          .AddApplicationInsightsTelemetry(Configuration.GetValue<string>("InstrumentationKey"));
    }

    public virtual void SetupAppSettings(IServiceCollection services)
    {
      services
        .Configure<AppSettings>(Configuration)
        .AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value);
    }

    public virtual AuthenticationBuilder SetupJwtAuthSchema(IServiceCollection services)
    {
      return services
          .AddAuthentication(options =>
          {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
          });
    }

    public virtual void SetupAuthentication(AuthenticationBuilder builder)
    {
      _ = builder
          .AddJwtBearer(options =>
          {
            options.Authority = Configuration.GetValue<string>("IdentityProvider");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
              NameClaimType = JwtNameClaimMapping,
              ValidAudiences = GetIdentityAudiences(),
            };
          });
    }

    public virtual string[] GetIdentityAudiences(AppSettings appSettings = null)
    {
      return (appSettings?.IdentityAudiences ?? Configuration.GetValue<string>("IdentityAudiences"))?.Split(',') ?? Array.Empty<string>();
    }

    public virtual void SetupDatabase(IServiceCollection services, string connectionString) { }


    public virtual void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
    }

    public virtual void SetupMiscDependencies(IServiceCollection services) { }

    public static IHostBuilder CreateDefaultHostBuilder<TStartup>() where TStartup : CoreWebStartup
    {
      return Host.CreateDefaultBuilder()
       .ConfigureWebHostDefaults(webBuilder =>
       {
         _ = webBuilder.UseStartup<TStartup>();
       });
    }

    public virtual void SetupSwaggerUI(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
    {
      foreach (var description in provider.ApiVersionDescriptions)
      {
        options.SwaggerEndpoint($"./swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
      }
      options.EnableDeepLinking();
      options.EnableFilter();
      options.RoutePrefix = string.Empty;
      options.HeadContent += "<meta name=\"robots\" content=\"none\" />";
      options.OAuthClientId(Configuration.GetValue<string>("SwaggerClientId"));
      options.OAuthClientSecret(Configuration.GetValue<string>("SwaggerClientSecret"));
      options.OAuthAppName(Configuration.GetValue<string>("SwaggerAppName"));
      options.OAuthScopeSeparator(" ");
      options.OAuthUsePkce();
    }

    public virtual void SetupSwaggerGen(SwaggerGenOptions options)
    {
      options.UseInlineDefinitionsForEnums();
      // add a custom operation filter which sets default values
      options.OperationFilter<SwaggerDefaultValues>();
      options.OperationFilter<SwaggerAuthorizeOperationFilter>();
    }

    private static OpenIdConfiguration OpenIdConfiguration;
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
    [SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>")]
    public virtual OpenIdConfiguration GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
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
  }
}
