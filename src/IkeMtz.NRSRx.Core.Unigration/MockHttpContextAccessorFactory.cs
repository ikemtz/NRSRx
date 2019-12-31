using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class MockHttpContextAccessorFactory
  {
    public string TestUser { get; set; } = "NRSRx Test User";
    public IHttpContextAccessor CreateAccessor()
    {
      var nameClaim = new Claim(ClaimTypes.Name, TestUser);
      var identity = new ClaimsIdentity(new[] { nameClaim });

      var principal = new ClaimsPrincipal(new[] { identity });
      var httpCtx = new Mock<HttpContext>();
      httpCtx.SetupGet(t => t.User).Returns(principal);
      var contextAccessor = new Mock<IHttpContextAccessor>();
      contextAccessor.SetupGet(t => t.HttpContext).Returns(httpCtx.Object);
      return contextAccessor.Object;
    }
  }
}
