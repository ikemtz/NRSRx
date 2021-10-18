using System.Reflection;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class StartUp_AppInsights : CoreWebApiStartup
  {
    public StartUp_AppInsights(IConfiguration configuration) : base(configuration)
    {
    }

    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) =>
      this.SetupDevelopmentApplicationInsights(services);


    public override string MicroServiceTitle => "";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
