using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Logging.Splunk.Tests
{
  public class StartUp_Splunk(IConfiguration configuration) : CoreWebApiStartup(configuration)
  {
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) =>
      app?.UseSerilog();


    public override OpenApiInfo ServiceInfo => new() { Title = "" };

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
