using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class StartUp_Splunk : StartUp_AppInsights
  {
    public StartUp_Splunk(IConfiguration configuration) : base(configuration)
    {
    }

    public override void SetupLogging(IServiceCollection services)
    {
      this.SetupSplunk();
    }
  }
}
