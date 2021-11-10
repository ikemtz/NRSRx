using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class CoreODataStartup : CoreWebStartup
  {
    public virtual int? MaxTop { get; set; }

    public abstract BaseODataModelProvider ODataModelProvider { get; }
    protected CoreODataStartup(IConfiguration configuration) : base(configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      _ = SetupCoreEndpointFunctionality(services)
          .AddApplicationPart(StartupAssembly);
      SetupAppSettings(services);
      SetupLogging(services);
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      SetupSwagger(services);
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, IApiVersionDescriptionProvider provider
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
      _ = app.UseAuthentication()
          .UseAuthorization();
      _ = app
          .UseSwagger()
          .UseSwaggerUI();
          //.UseSwaggerUI(options => SetupSwaggerUI(options, provider));
    }

    public IMvcBuilder SetupCoreEndpointFunctionality(IServiceCollection services)
    {
      var mvcBuilder = services
           .AddMvc();
     _ = services.AddApiVersioning(options => options.ReportApiVersions = true);
      _ = services.AddControllers()
          .AddOData(options =>
          {
            options.TimeZone = TimeZoneInfo.Utc;
            options.RouteOptions.EnableControllerNameCaseInsensitive = true;
            ODataModelProvider.GetModels().ToList().ForEach(x =>
            {
              _ = options.AddRouteComponents($"odata/{x.Key}", x.Value)
                .Select()
                .Filter()
                .Expand()
                .SetMaxTop(100)
                .Count()
                .OrderBy();
            });
          });
      return mvcBuilder;
    }

    public void SetupSwagger(IServiceCollection services)
    {
      _ = services
        .AddHttpClient()
        .AddTransient<IConfigureOptions<SwaggerGenOptions>>(serviceProvider => new ConfigureSwaggerOptions(serviceProvider, Configuration, this))
        .AddSwaggerGen(swaggerGenOptions =>
        {
          swaggerGenOptions.OperationFilter<ODataCommonOperationFilter>();
          SetupSwaggerGen(swaggerGenOptions);
        });
    }
  }
}
