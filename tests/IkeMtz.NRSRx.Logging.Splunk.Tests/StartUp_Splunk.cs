using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Logging.Splunk.Tests
{
  public class StartUp_Splunk : CoreWebApiStartup
  {
    public StartUp_Splunk(IConfiguration configuration) : base(configuration)
    {
    }
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) =>
      app?.UseSerilog();


    public override string MicroServiceTitle => "";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
