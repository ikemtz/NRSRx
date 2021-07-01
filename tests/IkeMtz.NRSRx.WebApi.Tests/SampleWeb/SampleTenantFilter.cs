using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Http;

namespace IkeMtz.NRSRx.WebApi.Tests.SampleWeb
{
  public class SampleTenantFilterAttribute : CoreTenantFilterAttribute
  {

    public override IEnumerable<string> GetUserTenants(HttpContext httpContext)
    {
      return httpContext.User.Claims.Where(c => c.Type.StartsWith("t")).Select(c => c.Value);
    }
  }
}
