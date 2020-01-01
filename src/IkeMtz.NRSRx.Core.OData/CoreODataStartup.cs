using System;
using System.Diagnostics.CodeAnalysis;
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
using Microsoft.Net.Http.Headers;
using Microsoft.OData;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class CoreODataStartup : CoreWebStartup
  {
    protected CoreODataStartup(IConfiguration configuration) : base(configuration)
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      SetupLogging(services);
      SetupDatabase(services, Configuration.GetValue<string>("SqlConnectionString"));
      SetupAuthentication(SetupJwtAuthSchema(services));
      SetupMiscDependencies(services);
      SetupCoreEndpointFunctionality(services)
          .AddApplicationPart(StartupAssembly);
      SetupSwagger(services);
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
      Justification = "VersionedODataModelBuilder is provided by the OData API version library.")]
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseAuthentication()
          .UseAuthorization();
      app
          .UseMvc(routeBuilder =>
          {
            routeBuilder.SetTimeZoneInfo(TimeZoneInfo.Utc);
            routeBuilder.Select().Expand().OrderBy().MaxTop(100).Filter().Count();

            var models = modelBuilder.GetEdmModels().ToList();
            var singleton = Microsoft.OData.ServiceLifetime.Singleton;
            routeBuilder.MapVersionedODataRoutes("odata-bypath", "odata/v{version:apiVersion}", models, oBuilder =>
            {
              oBuilder.AddService<ODataSerializerProvider, NrsrxODataSerializerProvider>(singleton);
            });
          });

      app
          .UseSwagger()
          .UseSwaggerUI(options =>
            {
              foreach (var description in provider.ApiVersionDescriptions)
              {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
              }
              options.RoutePrefix = string.Empty;
              options.OAuthClientId(Configuration.GetValue<string>("SwaggerClientId"));
              options.OAuthAppName(Configuration.GetValue<string>("SwaggerAppName"));
            });
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
           });
      services.AddApiVersioning(options => options.ReportApiVersions = true);

      services
        .AddOData()
        .EnableApiVersioning();
      services.AddODataApiExplorer(
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
  }
}
