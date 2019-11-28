using IkeMtz.NRSRx.Core.OData;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Tests.OData
{
  public class Startup : CoreODataStartup
  {
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    public override string MicroServiceTitle => "Test Web Application";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
