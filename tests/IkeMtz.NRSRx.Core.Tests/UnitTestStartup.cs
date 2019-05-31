using IkeMtz.NRSRx.Core.Unigration.WebApi;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core.Tests
{
    public class UnitTestStartup : CoreWebApiIntegrationTestStartup<Startup>
    {
        public UnitTestStartup(IConfiguration configuration) : base(new Startup(configuration))
        {
        }
    }
}
