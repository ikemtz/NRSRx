using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Web;

namespace NRSRx_WebApi_EF
{
  public class VersionDefinitions : IApiVersionDefinitions
  {
    public const string v1_0 = "1.0";

    [ExcludeFromCodeCoverage]
    public IEnumerable<string> Versions => new[] { v1_0 };
  }
}
