using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public abstract class CoreODataTestStartup<TStartup, TModelConfiguration> : CoreODataStartup
      where TStartup : CoreODataStartup
      where TModelConfiguration : IModelConfiguration, new()
  {
    protected TestContext TestContext { get; private set; }
    public TStartup Startup { get; private set; }
    protected CoreODataTestStartup(TStartup startup) : base(startup?.Configuration)
    {
      Startup = startup;
      base.MaxTop = startup.MaxTop;
    }

    public override string MicroServiceTitle => Startup.MicroServiceTitle;

    public override Assembly StartupAssembly => Startup.StartupAssembly;
    public override bool IncludeXmlCommentsInSwaggerDocs => Startup.IncludeXmlCommentsInSwaggerDocs;

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      Startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider)
    {
      modelBuilder.ModelConfigurations.Add(new TModelConfiguration());
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env, modelBuilder, provider);
    }
    public override void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
      _ = options.Filters.Add<TestContextResponseLoggerAttribute>(int.MaxValue);
      base.SetupMvcOptions(services, options);
    }

    public override void SetupSwaggerGen(SwaggerGenOptions options, string xmlPath = null)
    {
      base.SetupSwaggerGen(options, StartupAssembly.GetXmlCommentsFile());
    }
  }
}
