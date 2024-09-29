using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Abstract base class for setting up an OData test startup.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  public abstract class CoreODataTestStartup<TStartup> : CoreODataStartup
        where TStartup : CoreODataStartup
  {
    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected TestContext TestContext { get; private set; }

    /// <summary>
    /// Gets the startup instance.
    /// </summary>
    public TStartup Startup { get; private set; }

    /// <summary>
    /// Gets the OData model provider.
    /// </summary>
    public override BaseODataModelProvider ODataModelProvider => Startup.ODataModelProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreODataTestStartup{TStartup}"/> class.
    /// </summary>
    /// <param name="startup">The startup instance.</param>
    protected CoreODataTestStartup(TStartup startup) : base(startup.Configuration)
    {
      Startup = startup;
      base.MaxTop = startup?.MaxTop;
    }

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
    /// Gets the additional assembly XML document files.
    /// </summary>
    public override string[] AdditionalAssemblyXmlDocumentFiles => Startup.AdditionalAssemblyXmlDocumentFiles;

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
    /// Sets up health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="healthChecksBuilder">The health checks builder.</param>
    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
      Startup.SetupHealthChecks(services, healthChecksBuilder);
      base.SetupHealthChecks(services, healthChecksBuilder);
    }

    /// <summary>
    /// Sets up logging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="app">The application builder.</param>
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) { }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web host environment.</param>
    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env);
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
