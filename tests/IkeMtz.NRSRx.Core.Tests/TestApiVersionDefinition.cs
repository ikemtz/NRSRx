using IkeMtz.NRSRx.Core.Web;
using System.Collections.Generic;

namespace IkeMtz.NRSRx.Core.Tests
{
    public class TestApiVersionDefinitions : IApiVersionDefinitions
    {
        public const string v1_0 = "1.0";

        public IEnumerable<string> Versions => new[] { v1_0 };
    }
}
