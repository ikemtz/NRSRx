using Microsoft.AspNetCore.Http;

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
      return HttpContextAccessor.HttpContext?.User?.Identity?.Name ?? anonymousValue;
    }
  }
}
