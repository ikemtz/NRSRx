using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
    public class CoreODataUnigrationTestStartup<Startup, ModelConfiguration> 
        : CoreODataTestStartup<Startup, ModelConfiguration>
        where Startup : CoreODataStartup
        where ModelConfiguration : IModelConfiguration, new()
    {
        public CoreODataUnigrationTestStartup(Startup startup) : base(startup)
        {
        }

        public override void SetupAuthentication(AuthenticationBuilder builder)
        {
            builder.SetupTestAuthentication(Configuration, TestContext);
        }
    }
}
