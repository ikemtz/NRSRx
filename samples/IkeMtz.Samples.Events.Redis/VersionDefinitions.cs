using IkeMtz.NRSRx.Core.Web;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IkeMtz.Samples.Events.Redis
{
    public class VersionDefinitions : IApiVersionDefinitions
    {
        public const string v1_0 = "1.0";

        [ExcludeFromCodeCoverage]
        public IEnumerable<string> Versions => new[] { v1_0 };
    }
}
