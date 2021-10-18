using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class StartUp_Splunk : StartUp_AppInsights
  {
    public StartUp_Splunk(IConfiguration configuration) : base(configuration)
    {
    }
    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) =>
      app.UseSerilog();
  }
}
