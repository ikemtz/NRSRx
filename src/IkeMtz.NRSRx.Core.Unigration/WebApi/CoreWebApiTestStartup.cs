using System.Net.Http;
using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public abstract class CoreWebApiTestStartup<TStartup> : CoreWebApiStartup
      where TStartup : CoreWebApiStartup
  {
    public TStartup Startup { get; private set; }
    protected CoreWebApiTestStartup(TStartup startup) : base(startup?.Configuration)
    {
      this.Startup = startup;
    }

    public override string MicroServiceTitle => Startup.MicroServiceTitle;

    public override Assembly StartupAssembly => Startup.StartupAssembly;
    public override bool IncludeXmlCommentsInSwaggerDocs => Startup.IncludeXmlCommentsInSwaggerDocs;
    public override string[] AdditionalAssemblyXmlDocumentFiles => Startup.AdditionalAssemblyXmlDocumentFiles;

    protected TestContext TestContext { get; private set; }

    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder builder)
    {
      Startup.SetupHealthChecks(services, builder);
      base.SetupHealthChecks(services, builder);
    }

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      Startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) { }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env, provider);
    }
    public override void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
      _ = options.Filters.Add<TestContextResponseLoggerAttribute>(int.MaxValue);
      base.SetupMvcOptions(services, options);
    }
    public override OpenIdConfiguration GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
    {
      return new OpenIdConfiguration
      {
        AuthorizeEndpoint = "https://demo.identityserver.io/connect/authorize",
        TokenEndpoint = $"https://demo.identityserver.io/connect/token",
      };
    }

    public override void SetupSwaggerGen(SwaggerGenOptions options, string xmlPath = null)
    {
      base.SetupSwaggerGen(options, StartupAssembly.GetXmlCommentsFile());
    }
  }
}
