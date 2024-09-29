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
  /// <summary>
  /// Provides a base class for setting up a test startup for Web API projects.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  public abstract class CoreWebApiTestStartup<TStartup>(TStartup startup) : CoreWebApiStartup(startup.Configuration)
        where TStartup : CoreWebApiStartup
  {
    /// <summary>
    /// Gets the startup instance.
    /// </summary>
    public TStartup Startup { get; private set; } = startup;

    /// <summary>
    /// Gets the service title.
    /// </summary>
    public override string ServiceTitle => Startup.ServiceTitle;

    /// <summary>
    /// Gets the startup assembly.
    /// </summary>
    public override Assembly StartupAssembly => Startup.StartupAssembly;

    /// <summary>
    /// Gets a value indicating whether to include XML comments in Swagger documentation.
    /// </summary>
    public override bool IncludeXmlCommentsInSwaggerDocs => Startup.IncludeXmlCommentsInSwaggerDocs;

    /// <summary>
    /// Gets additional assembly XML document files.
    /// </summary>
    public override string[] AdditionalAssemblyXmlDocumentFiles => Startup.AdditionalAssemblyXmlDocumentFiles;

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected TestContext TestContext { get; private set; }

    /// <summary>
    /// Sets up health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="builder">The health checks builder.</param>
    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder builder)
    {
      Startup.SetupHealthChecks(services, builder);
      base.SetupHealthChecks(services, builder);
    }

    /// <summary>
    /// Sets up miscellaneous dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public override void SetupMiscDependencies(IServiceCollection services)
    {
      Startup.SetupMiscDependencies(services);
      base.SetupMiscDependencies(services);
    }

    /// <summary>
    /// Sets up logging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="app">The application builder.</param>
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) { }

    /// <summary>
    /// Configures the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web host environment.</param>
    /// <param name="provider">The API version description provider.</param>
    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env, provider);
    }

    /// <summary>
    /// Sets up MVC options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The MVC options.</param>
    public override void SetupMvcOptions(IServiceCollection services, MvcOptions options)
    {
      _ = options.Filters.Add<TestContextResponseLoggerAttribute>(int.MaxValue);
      base.SetupMvcOptions(services, options);
    }

    /// <summary>
    /// Gets the OpenID configuration.
    /// </summary>
    /// <param name="clientFactory">The HTTP client factory.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <returns>The OpenID configuration.</returns>
    public override OpenIdConfiguration GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
    {
      return new OpenIdConfiguration
      {
        AuthorizeEndpoint = "https://demo.identityserver.io/connect/authorize",
        TokenEndpoint = $"https://demo.identityserver.io/connect/token",
      };
    }

    /// <summary>
    /// Sets up Swagger generation options.
    /// </summary>
    /// <param name="options">The Swagger generation options.</param>
    /// <param name="xmlPath">The XML path for comments.</param>
    public override void SetupSwaggerGen(SwaggerGenOptions options, string? xmlPath = null)
    {
      base.SetupSwaggerGen(options, StartupAssembly.GetXmlCommentsFile());
    }
  }
}
