using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace IkeMtz.NRSRx.Core.Web
{
  public abstract class CoreWebStartup
  {
    public abstract string MicroServiceTitle { get; }
    public abstract Assembly StartupAssembly { get; }
    public virtual string JwtNameClaimMapping { get; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
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

    protected virtual string[] GetIdentityAudiences()
    {
      return Configuration.GetValue<string>("IdentityAudiences")?.Split(',') ?? Array.Empty<string>();
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
      options.OAuthAppName(Configuration.GetValue<string>("SwaggerAppName"));
    }

    public virtual void SetupSwaggerGen(SwaggerGenOptions options)
    {
      options.UseInlineDefinitionsForEnums();
      // add a custom operation filter which sets default values
      options.OperationFilter<SwaggerDefaultValues>();
      var audiences = GetIdentityAudiences();
      var swaggerIdentityProviderUrl = Configuration.GetValue<string>("SwaggerIdentityProviderUrl");
      if (audiences.Any() && !string.IsNullOrWhiteSpace(swaggerIdentityProviderUrl))
      {
        var audience = audiences.FirstOrDefault();

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.OAuth2,
          In = ParameterLocation.Header,
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Scheme = JwtBearerDefaults.AuthenticationScheme,
          Flows = new OpenApiOAuthFlows
          {
            Implicit = new OpenApiOAuthFlow
            {
              AuthorizationUrl = new Uri($"{swaggerIdentityProviderUrl}authorize?audience={audience}"),
              Scopes = SwaggerScopes,
            },
          }
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme}
                        },
                       Array.Empty<string>()
                    }
                });
      }
    }
  }
}
