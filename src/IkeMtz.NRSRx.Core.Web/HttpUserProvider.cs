using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace IkeMtz.NRSRx.Core.Web
{
  public class HttpUserProvider : ICurrentUserProvider
  {
    public IHttpContextAccessor HttpContextAccessor { get; }

    public HttpUserProvider(IHttpContextAccessor httpContextAccessor)
    {
      HttpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId(string? anonymousValue = null) //NOSONAR
    {
      var user = HttpContextAccessor.HttpContext?.User;
      return user?.Identity?.Name ?? anonymousValue ?? user?.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Sub)?.Value;
    }
  }
}
