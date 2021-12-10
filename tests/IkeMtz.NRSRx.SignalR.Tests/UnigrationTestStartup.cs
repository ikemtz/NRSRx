using IkeMtz.NRSRx.Core.Unigration.SignalR;
using IkeMtz.Samples.SignalR;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.SignalR.Tests
{
  public class UnigrationTestStartup : CoreSignalrUnigrationTestStartup<Startup>
  {
    public UnigrationTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }
  }
}
