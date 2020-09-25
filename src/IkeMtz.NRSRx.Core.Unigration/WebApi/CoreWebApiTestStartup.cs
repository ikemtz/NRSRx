using System;
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

    protected TestContext TestContext { get; private set; }

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      Startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

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
      var path = StartupAssembly.CodeBase
        .Replace(".dll", ".xml", System.StringComparison.InvariantCultureIgnoreCase)
        //This is here to work around an issue on Azure Devops build agents not finding the .xml file.
        .Replace(@"\$(BuildConfiguration)\", @"\Debug\", System.StringComparison.InvariantCultureIgnoreCase)
        ;
      base.SetupSwaggerGen(options, path);
    }
  }
}
