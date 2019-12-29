using IkeMtz.NRSRx.Core.OData;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Tests.OData
{
  public class Startup : CoreODataStartup
  {

    public override string MicroServiceTitle => "Test WebApi Application";

    public override Assembly StartupAssembly => this.GetType().Assembly;

    public Startup(IConfiguration configuration) : base(configuration)
    {
    }
  }
}
