using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class StartUp_Splunk : StartUp_AppInsights
  {
    public StartUp_Splunk(IConfiguration configuration) : base(configuration)
    {
    }
    public override void ConfigureLogging(IApplicationBuilder app)
    {
      base.ConfigureLogging(app);
      app.UseSerilog();
    }
  }
}
