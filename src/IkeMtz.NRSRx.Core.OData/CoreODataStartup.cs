using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OData;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class CoreODataStartup : CoreWebStartup
  {
    public virtual int? MaxTop { get; set; }
    protected CoreODataStartup(IConfiguration configuration) : base(configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      SetupAppSettings(services);
      SetupLogging(services);
      SetupDatabase(services, Configuration.GetValue<string>("DbConnectionString"));
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      _ = SetupCoreEndpointFunctionality(services)
          .AddApplicationPart(StartupAssembly);
      SetupSwagger(services);
    }

    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider)
    {
      if (env.IsDevelopment())
      {
        _ = app.UseDeveloperExceptionPage();
      }
      else
      {
        _ = app.UseHsts();
      }

      _ = app.UseAuthentication()
          .UseAuthorization();
      _ = app
          .UseRouting()
          .UseEndpoints(endpoints =>
          { 
            var models = modelBuilder.GetEdmModels().ToList();
            var singleton = Microsoft.OData.ServiceLifetime.Singleton;
            _ = endpoints
            .SetTimeZoneInfo(TimeZoneInfo.Utc)
            .Select()
            .Expand()
            .OrderBy()
            .MaxTop(MaxTop ?? 100)
            .Filter()
            .Count()
           .MapVersionedODataRoute("odata-bypath", "odata/v{version:apiVersion}", models, oBuilder =>
           {
             _ = oBuilder.AddService<ODataSerializerProvider, NrsrxODataSerializerProvider>(singleton);
           });
          })
          .UseSwagger()
          .UseSwaggerUI(options => SetupSwaggerUI(options, provider));
    }

    public IMvcBuilder SetupCoreEndpointFunctionality(IServiceCollection services)
    {
      var mvcBuilder = services
           .AddMvc(options =>
           {
             options.EnableEndpointRouting = false;
             options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
             foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
             {
               outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
             }
             foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
             {
               inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
             }
             SetupMvcOptions(services, options);
           })
           .AddNewtonsoftJson();
      _ = services.AddApiVersioning(options => options.ReportApiVersions = true)
          .AddOData()
          .EnableApiVersioning();
      _ = services.AddODataApiExplorer(
          options =>
          {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
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
