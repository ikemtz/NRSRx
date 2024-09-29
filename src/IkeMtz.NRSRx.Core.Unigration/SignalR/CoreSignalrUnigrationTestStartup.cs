using IkeMtz.NRSRx.Core.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.SignalR
{
  /// <summary>
  /// Abstract base class for setting up a SignalR unigration test startup.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  /// <remarks>
  /// Initializes a new instance of the <see cref="CoreSignalrUnigrationTestStartup{TStartup}"/> class.
  /// </remarks>
  /// <param name="startup">The startup instance.</param>
  public abstract class CoreSignalrUnigrationTestStartup<TStartup>(TStartup startup) : CoreSignalrStartup(startup.Configuration)
          where TStartup : CoreSignalrStartup
  {
    /// <summary>
    /// Gets the startup instance.
    /// </summary>
    public TStartup Startup { get; private set; } = startup;

    /// <summary>
    /// Gets the test context.
    /// </summary>
    protected TestContext TestContext { get; private set; }

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
    /// Maps SignalR hubs to endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public override void MapHubs(IEndpointRouteBuilder endpoints) =>
      Startup.MapHubs(endpoints);

    /// <summary>
    /// Sets up authentication.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder) =>
      builder.SetupTestAuthentication(Configuration, TestContext);
  }
}
