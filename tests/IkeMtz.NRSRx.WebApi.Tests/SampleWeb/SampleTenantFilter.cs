using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IkeMtz.NRSRx.Core.Web;

namespace IkeMtz.NRSRx.WebApi.Tests.SampleWeb
{
  public class SampleTenantFilterAttribute : CoreTenantFilterAttribute
  {
    public SampleTenantFilterAttribute() { }

    public override IEnumerable<string> GetUserTenants(IEnumerable<Claim> claims)
    {
      return claims.Where(c => c.Type.StartsWith("t")).Select(c => c.Value);
    }
  }
}
