using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class MockHttpContextAccessorFactory
  {
    public static string TestUser { get; set; } = "NRSRx Test User";
    public static IHttpContextAccessor CreateAccessor()
    {
      var nameClaim = new Claim(ClaimTypes.Name, TestUser);
      var identity = new ClaimsIdentity(new[] { nameClaim });

      var principal = new ClaimsPrincipal(new[] { identity });
      var httpCtx = new Mock<HttpContext>();
      _ = httpCtx.SetupGet(t => t.User).Returns(principal);
      var contextAccessor = new Mock<IHttpContextAccessor>();
      _ = contextAccessor.SetupGet(t => t.HttpContext).Returns(httpCtx.Object);
      return contextAccessor.Object;
    }
  }
}
