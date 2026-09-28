using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class StartUp_AppInsights(IConfiguration configuration) : CoreWebApiStartup(configuration)
  {
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) =>
      this.SetupDevelopmentApplicationInsights(services);

    public override OpenApiInfo ServiceInfo => new() { Title = "App Insights integrated with sample service" };

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
