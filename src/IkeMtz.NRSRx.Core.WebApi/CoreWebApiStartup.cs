using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

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
      SetupAppSettings(services);
      SetupLogging(services);
      SetupSwagger(services);
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupPublishers(services);
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      SetupHealthChecks(services);
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
      SetupLogging(null, app);
      _ = app
       .UseRouting()
       .UseAuthentication()
       .UseAuthorization();
      if (!DisableSwagger && Configuration?.GetValue<bool>("DisableSwagger", false) != true)
      {
        _ = app
        .UseSwagger()
        .UseSwaggerUI(options => SetupSwaggerUI(options, provider));
      }
      _ = app
      .UseEndpoints(endpoints =>
     {
       _ = endpoints.MapHealthChecks("/health");
       _ = endpoints.MapControllers();
     });
    }

    public virtual void SetupSwaggerUI(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
    {
      var swaggerJsonRoutePrefix = string.IsNullOrEmpty(SwaggerUiRoutePrefix) ? "./swagger/" : "./";
      foreach (var groupName in provider.ApiVersionDescriptions
        .Select(s => s.GroupName))
      {
        options.SwaggerEndpoint($"{swaggerJsonRoutePrefix}{groupName}/swagger.json", groupName.ToUpperInvariant());
      }
      SetupSwaggerCommonUi(options);
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
           .AddXmlSerializerFormatters();
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

    public virtual void SetupSwagger(IServiceCollection services)
    {
      _ = services
        .AddHttpClient()
        .AddTransient<IConfigureOptions<SwaggerGenOptions>>(serviceProvider => new ConfigureSwaggerOptions(serviceProvider, Configuration, this))
        .AddSwaggerGen(options =>
        {
          SetupSwaggerGen(options);
        });
    }

    public virtual void SetupPublishers(IServiceCollection services) { }
  }
}
