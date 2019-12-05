using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IkeMtz.NRSRx.Core.Authorization
{
  public abstract class BaseActionFilterAttribute : ActionFilterAttribute
  {
    public const string DefaultPermissionClaimType = "permissions";
    public const string DefaultScopeClaimType = "scope";
    protected readonly string[] allowedPermissions;
    protected readonly string permissionClaimType;
    protected readonly char permissionClaimSeperator;
    protected readonly bool allowScopes;
    protected readonly string scopeClaimType;

    protected BaseActionFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
    {
      this.allowedPermissions = allowedPermissions;
      this.permissionClaimSeperator = permissionClaimSeperator;
      this.permissionClaimType = permissionClaimType;
      this.allowScopes = allowScopes;
      this.scopeClaimType = scopeClaimType;
    }

    public bool HasPermission(ActionExecutingContext context)
    {
      if (HasMatchingPermissionClaim(permissionClaimType, context.HttpContext.User.Claims, x => x))
      {
        return true;
      }
      return allowScopes && HasMatchingPermissionClaim(scopeClaimType, context.HttpContext.User.Claims, x => x.SelectMany(t=> t.Split(' ')));
    }

    public bool HasMatchingPermissionClaim(string type, IEnumerable<Claim> claims, Func<IEnumerable<string>, IEnumerable<string>> permissionsSeperator)
    {
      var permissions = claims
        .Where(f => type.Equals(f?.Type, StringComparison.CurrentCultureIgnoreCase))
        .Select(t=> t.Value)
        .Where(t => !string.IsNullOrWhiteSpace(t));
      if (permissions.Any())
      {
        var userPermissions = permissionsSeperator(permissions);
        return userPermissions.Any(a => allowedPermissions.Any(t => a.Equals(t, StringComparison.InvariantCultureIgnoreCase)));
      }
      return false;
    }
  }
}
