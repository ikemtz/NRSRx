using IkeMtz.NRSRx.Core.Unigration.SignalR;
using IkeMtz.Samples.SignalR;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.SignalR.Tests
{
  public class UnigrationTestStartup(IConfiguration configuration) : CoreSignalrUnigrationTestStartup<Startup>(new Startup(configuration))
  {
  }
}
