using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public abstract class CoreODataTestStartup<Startup, ModelConfiguration> : CoreODataStartup
      where Startup : CoreODataStartup
      where ModelConfiguration : IModelConfiguration, new()
  {
    protected TestContext TestContext { get; private set; }
    protected readonly Startup _startup;
    protected CoreODataTestStartup(Startup startup) : base(startup.Configuration)
    {
      _startup = startup;
    }

    public override string MicroServiceTitle => _startup.MicroServiceTitle;

    public override Assembly StartupAssembly => _startup.StartupAssembly;

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      _startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider)
    {
      modelBuilder.ModelConfigurations.Add(new ModelConfiguration());
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env, modelBuilder, provider);
    }
    public override void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
      _ = options.Filters.Add<TestContextResponseLoggerAttribute>(int.MaxValue);
      base.SetupMvcOptions(services, options);
    }
  }
}
