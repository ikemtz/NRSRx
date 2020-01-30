using System.Collections.Generic;
using System.Security.Claims;
using IkeMtz.NRSRx.Core.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class PermissionsTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void ScopedPermission_Fail_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultScopeClaimType, "openid profile email offline_access") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" }, allowScopes: true);
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Scoped_Permissions_Pass_Many2Many_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultScopeClaimType, "openid profile email offline_access v1:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Permissions_Fail_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v4:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v3:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Permissions_Fail_NoPerms_Test()
    {
      var claims = System.Array.Empty<Claim>();
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Permissions_Pass_Many2Many_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v3:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Permissions_Pass_OneOfMany_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v4:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v6:w:data")};
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data", "v2:2:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Permissions_Pass_Single_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(new[] { "v1:w:data" });
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    private ActionExecutingContext ActionExecutingContextFactory(IEnumerable<Claim> userClaims)
    {
      var identity = new ClaimsIdentity(userClaims);
      var httpCtx = new DefaultHttpContext
      {
        User = new ClaimsPrincipal(identity)
      };
      var actCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
      var filterMetaData = new List<IFilterMetadata>();
      var actionArguments = new Dictionary<string, object>();
      return new ActionExecutingContext(actCtx, filterMetaData, actionArguments, null);
    }
  }
}
