using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides a factory for creating mock <see cref="IHttpContextAccessor"/> instances for testing purposes.
  /// </summary>
  public static class MockHttpContextAccessorFactory
  {
    /// <summary>
    /// Gets or sets the test user name.
    /// </summary>
    public static string TestUser { get; set; } = "NRSRx Test User";

    /// <summary>
    /// Creates a mock <see cref="IHttpContextAccessor"/> with a predefined user.
    /// </summary>
    /// <returns>A mock <see cref="IHttpContextAccessor"/> instance.</returns>
    public static IHttpContextAccessor CreateAccessor()
    {
      var nameClaim = new Claim(ClaimTypes.Name, TestUser);
      var identity = new ClaimsIdentity([nameClaim]);

      var principal = new ClaimsPrincipal([identity]);
      var httpCtx = new Mock<HttpContext>();
      _ = httpCtx.SetupGet(t => t.User).Returns(principal);
      var contextAccessor = new Mock<IHttpContextAccessor>();
      _ = contextAccessor.SetupGet(t => t.HttpContext).Returns(httpCtx.Object);
      return contextAccessor.Object;
    }
  }
}
