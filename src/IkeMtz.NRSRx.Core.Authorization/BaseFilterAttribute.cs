using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IkeMtz.NRSRx.Core.Authorization
{
  public class BaseActionFilterAttribute : ActionFilterAttribute
  {
    public const string DefaultPermissionClaimType = "permissions";
    public const string DefaultScopeClaimType = "scope";
    protected readonly string[] allowedPermissions;
    protected readonly string permissionClaimType;
    protected readonly char permissionClaimSeperator;
    protected readonly bool allowScopes;
    protected readonly string scopeClaimType;

    public BaseActionFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
    {
      this.allowedPermissions = allowedPermissions;
      this.permissionClaimSeperator = permissionClaimSeperator;
      this.permissionClaimType = permissionClaimType;
      this.allowScopes = allowScopes;
      this.scopeClaimType = scopeClaimType;
    }

    protected bool HasPermission(ActionExecutingContext context)
    {
      if (HasMatchingPermissionClaim(permissionClaimType, context.HttpContext.User.Claims, x => JsonConvert.DeserializeObject<string[]>(x)))
      {
        return true;
      }
      return allowScopes && HasMatchingPermissionClaim(scopeClaimType, context.HttpContext.User.Claims, x=> x.Split(' '));
    }

    private bool HasMatchingPermissionClaim(string type, IEnumerable<Claim> claims, Func<string, string[]> permissionsSeperator)
    {
      var claim = claims.FirstOrDefault(f => type.Equals(f?.Type, StringComparison.CurrentCultureIgnoreCase));
      if (claim != null)
      {
        var userPermissions = permissionsSeperator(claim.Value);
        return userPermissions.Any(a => allowedPermissions.Any(t => a.Equals(t, StringComparison.InvariantCultureIgnoreCase)));
      }
      return false;
    }
  }
}
