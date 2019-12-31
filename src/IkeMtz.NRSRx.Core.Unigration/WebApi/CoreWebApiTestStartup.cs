using System.Reflection;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public abstract class CoreWebApiTestStartup<Startup> : CoreWebApiStartup
      where Startup : CoreWebApiStartup
  {
    protected readonly Startup startup;
    protected CoreWebApiTestStartup(Startup startup) : base(startup.Configuration)
    {
      this.startup = startup;
    }

    public override string MicroServiceTitle => startup.MicroServiceTitle;

    public override Assembly StartupAssembly => startup.StartupAssembly;

    protected TestContext TestContext { get; private set; }

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
      TestContext = app.ApplicationServices.GetService<TestContext>();
      app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env, provider);
    }
    public override void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
      options.Filters.Add<TestContextResponseLoggerAttribute>(int.MaxValue);
      base.SetupMvcOptions(services, options);
    }
  }
}
