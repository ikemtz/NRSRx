using System;
using System.Collections.Generic;
using System.Security.Claims;
using IkeMtz.NRSRx.Core.Authorization;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class PermissionsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ScopedPermission_Fail_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultScopeClaimType, "openid profile email offline_access") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"], allowScopes: true);
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Scoped_Permissions_Pass_Many2Many_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultScopeClaimType, "openid profile email offline_access v1:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Permissions_Fail_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v4:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v3:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Permissions_Fail_NoPerms_Test()
    {
      var claims = Array.Empty<Claim>();
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsInstanceOfType(ctx.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Permissions_Pass_Many2Many_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v3:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Permissions_Pass_OneOfMany_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v4:w:data"),
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v6:w:data")};
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data", "v2:2:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Permissions_Pass_Single_Test()
    {
      var claims = new[] {
        new Claim(BaseActionFilterAttribute.DefaultPermissionClaimType, "v1:w:data") };
      var ctx = ActionExecutingContextFactory(claims);
      var attrib = new PermissionsFilterAttribute(["v1:w:data"]);
      attrib.OnActionExecuting(ctx);
      Assert.IsNull(ctx.Result);
    }

    public static ActionExecutingContext ActionExecutingContextFactory(IEnumerable<Claim> userClaims)
    {
      var identity = new ClaimsIdentity(userClaims);
      var httpCtx = new DefaultHttpContext
      {
        User = new ClaimsPrincipal(identity)
      };
      var actCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
      var filterMetaData = new List<IFilterMetadata>();
      var actionArguments = new Dictionary<string, object?>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
      return new ActionExecutingContext(actCtx, filterMetaData, actionArguments, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
  }
}
