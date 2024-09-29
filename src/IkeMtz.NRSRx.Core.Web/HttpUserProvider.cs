using Microsoft.AspNetCore.Http;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Provides the current user information from the HTTP context.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="HttpUserProvider"/> class.
  /// </remarks>
  /// <param name="httpContextAccessor">The HTTP context accessor.</param>
  public class HttpUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
  {
    /// <summary>
    /// Gets the HttpContextAccessor.
    /// </summary>
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;

    /// <summary>
    /// Gets the current user ID from the HTTP context.  If Http request is not authenticated, it will return null.
    /// </summary>
    /// <param name="anonymousValue">The value to return if the user is not authenticated.</param>
    /// <returns>The current user ID, or the specified anonymous value if the user is not authenticated.</returns>
    public string? GetCurrentUserId(string? anonymousValue = null) //NOSONAR
    {
      var user = HttpContextAccessor.HttpContext?.User;
      return user?.Identity?.Name ?? anonymousValue;
    }
  }
}
