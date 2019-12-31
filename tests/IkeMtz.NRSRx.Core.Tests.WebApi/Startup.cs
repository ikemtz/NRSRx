using System.Reflection;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core.TestWebApp
{
  public class Startup : CoreWebApiStartup
  {
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    public override string MicroServiceTitle => "Test WebApi Application";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
