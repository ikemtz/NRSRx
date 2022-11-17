using System.Security.Claims;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class HttpUserProviderTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public void ValidateUserSubClaim()
    {
      //arrange
      var userName = "Subjected User";
      var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userName) };
      var identity = new ClaimsIdentity(claims);
      var principal = new ClaimsPrincipal(identity);
      var context = new Mock<HttpContext>();
      _ = context.Setup(t => t.User)
          .Returns(principal);
      var moqAccessor = new Mock<IHttpContextAccessor>();
      _ = moqAccessor.Setup(m => m.HttpContext).Returns(
         context.Object);
      var provider = new HttpUserProvider(moqAccessor.Object);

      //act
      var result = provider.GetCurrentUserId();

      //assert
      Assert.AreEqual(result, userName);
    }
  }
}
