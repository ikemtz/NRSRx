using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApiContrib.Core.Formatter.MessagePack;

namespace IkeMtz.NRSRx.Core.WebApi
{
  public abstract class CoreWebApiStartup : CoreWebStartup
  {
    protected CoreWebApiStartup(IConfiguration configuration) : base(configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      SetupLogging(services);
      SetupSwagger(services);
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupPublishers(services);
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      _ = SetupCoreEndpointFunctionality(services)
         .AddApplicationPart(StartupAssembly)
         .AddControllersAsServices();
      _ = services.AddControllers();
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
      if (env.IsDevelopment())
      {
        _ = app.UseDeveloperExceptionPage();
      }
      else
      {
        _ = app.UseHsts();
      }
      _ = app
       .UseRouting()
       .UseAuthentication()
       .UseAuthorization()
       .UseSwagger()
       .UseSwaggerUI(options =>
       {
         // build a swagger endpoint for each discovered API version
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
       })
       .UseEndpoints(endpoints =>
      {
        _ = endpoints.MapControllers();
      });
    }

    public IMvcBuilder SetupCoreEndpointFunctionality(IServiceCollection services)
    {
      var builder = services
           .AddMvc(options =>
           {
             options.EnableEndpointRouting = true;
             options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
             SetupMvcOptions(services, options);
           })
           .AddNewtonsoftJson(options =>
           {
             options
             .UseCamelCasing(true)
             .SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

             options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           })
           .AddMessagePackFormatters()
           .AddXmlSerializerFormatters()
           .SetCompatibilityVersion(CompatibilityVersion.Latest);
      _ = services
           .AddApiVersioning(options =>
           {
             // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
             options.ReportApiVersions = true;
             options.ApiVersionReader = new UrlSegmentApiVersionReader();
           })
           .AddVersionedApiExplorer(options =>
           {
             // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
             // note: the specified format code will format the version as "'v'major[.minor][-status]"
             options.GroupNameFormat = "'v'VVV";

             // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
             // can also be used to control the format of the API version in route templates
             options.SubstituteApiVersionInUrl = true;
           });
      return builder;
    }

    public void SetupSwagger(IServiceCollection services)
    {
      _ = services
        .AddTransient<IConfigureOptions<SwaggerGenOptions>>(serviceProvider => new ConfigureSwaggerOptions(serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>(), this))
        .AddSwaggerGen(options =>
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
        });
    }

    public virtual void SetupPublishers(IServiceCollection services) { }
  }
}
