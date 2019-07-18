using Microsoft.AspNetCore.Mvc.Filters;
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
      if (HasMatchingPermissionClaim(permissionClaimType, context.HttpContext.User.Claims, permissionClaimSeperator))
      {
        return true;
      }
      return allowScopes && HasMatchingPermissionClaim(scopeClaimType, context.HttpContext.User.Claims, ' ');
    }

    private bool HasMatchingPermissionClaim(string type, IEnumerable<Claim> claims, char sepereator)
    {
      var claim = claims.FirstOrDefault(f => type.Equals(f?.Type, StringComparison.CurrentCultureIgnoreCase));
      if (claim != null)
      {
        var userPermissions = claim.Value.Split(sepereator).Select(t => t);
        return userPermissions.Any(a => allowedPermissions.Any(t => a.Equals(t, StringComparison.InvariantCultureIgnoreCase)));
      }
      return false;
    }
  }
}
