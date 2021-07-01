using System.Linq;
using IkeMtz.NRSRx.Core.Web;

namespace IkeMtz.NRSRx.WebApi.Tests.SampleWeb
{
  public class SampleTenantFilterAttribute : CoreTenantFilterAttribute
  {
    public SampleTenantFilterAttribute() : base(
      (claims) => claims.Where(c => c.Type.StartsWith("t")).Select(c => c.Value))
    {
    }
  }
}
