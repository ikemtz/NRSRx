using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class CoreODataStartup : CoreWebStartup
  {
    public virtual int? MaxTop { get; set; } = 100;

    public abstract BaseODataModelProvider ODataModelProvider { get; }
    protected CoreODataStartup(IConfiguration configuration) : base(configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      if (StartupAssembly != null)
      {
        _ = SetupCoreEndpointFunctionality(services)
            .AddApplicationPart(StartupAssembly);
      }
      SetupAppSettings(services);
      SetupLogging(services);
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      SetupSwagger(services);
      var healthCheckBuilder = services.AddHealthChecks();
      SetupHealthChecks(services, healthCheckBuilder);
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        _ = app
          .UseDeveloperExceptionPage()
          .UseODataRouteDebug();
      }
      else
      {
        _ = app.UseHsts();
      }
      SetupLogging(null, app);
      _ = app.UseRouting();
      _ = app.UseAuthentication()
          .UseAuthorization();
      if (!DisableSwagger && Configuration?.GetValue<bool>("DisableSwagger", false) != true)
      {
        _ = app
            .UseSwagger()
            .UseSwaggerUI(SetupSwaggerUI);
      }
      _ = app.UseEndpoints(endpoints =>
      {
        _ = endpoints.MapHealthChecks("/healthz");
        _ = endpoints.MapControllers();
      });
    }

    public virtual void SetupSwaggerUI(SwaggerUIOptions options)
    {
      var swaggerJsonRoutePrefix = string.IsNullOrEmpty(SwaggerUiRoutePrefix) ? "./swagger/" : "./";
      foreach (var groupName in ODataModelProvider.GetODataVersions().Select(t => t.GroupName))
      {
        options.SwaggerEndpoint(
          $"{swaggerJsonRoutePrefix}{groupName}/swagger.json",
          groupName.ToUpperInvariant());
      }
      SetupSwaggerCommonUi(options);
    }

    public virtual IMvcBuilder SetupCoreEndpointFunctionality(IServiceCollection services)
    {
      var mvcBuilder = services
           .AddMvc();
      _ = services.AddApiVersioning(options => options.ReportApiVersions = true);
      _ = services.AddControllers()
          .AddOData(options =>
          {
            options.TimeZone = TimeZoneInfo.Utc;
            options.RouteOptions.EnableControllerNameCaseInsensitive = true;
            ODataModelProvider.EdmModels.ToList().ForEach(x =>
            {
              options.AddRouteComponents($"odata/{x.Key.GroupName}",
                  x.Value,
                  builder => builder.AddSingleton<IODataSerializerProvider, NrsrxODataSerializerProvider>())
               .EnableQueryFeatures(MaxTop)
               .EnableAttributeRouting = true;
            });
          });
      return mvcBuilder;
    }

    public virtual void SetupSwagger(IServiceCollection services)
    {
      _ = services
        .AddHttpClient()
        .AddSingleton<IODataVersionProvider>((x) => this.ODataModelProvider)
        .AddTransient<IConfigureOptions<SwaggerGenOptions>>(serviceProvider => new ConfigureSwaggerOptions(serviceProvider, Configuration, this))
        .AddSwaggerGen(swaggerGenOptions =>
        {
          swaggerGenOptions.OperationFilter<ODataCommonOperationFilter>();
          swaggerGenOptions.DocumentFilter<ODataCommonDocumentFilter>();
          SetupSwaggerGen(swaggerGenOptions);
        });
    }
  }
}
