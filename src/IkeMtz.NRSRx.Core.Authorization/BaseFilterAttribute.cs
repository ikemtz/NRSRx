using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Authorization
{
  public abstract class BaseActionFilterAttribute : ActionFilterAttribute
  {
    public const string DefaultPermissionClaimType = "permissions";
    public const string DefaultScopeClaimType = "scope";
    public string[] AllowedPermissions { get; private set; }
    public string PermissionClaimType { get; private set; }
    public char PermissionClaimSeperator { get; private set; }
    public bool AllowScopes { get; private set; }
    public string ScopeClaimType { get; private set; }

    protected BaseActionFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
    {
      this.AllowedPermissions = allowedPermissions;
      this.PermissionClaimSeperator = permissionClaimSeperator;
      this.PermissionClaimType = permissionClaimType;
      this.AllowScopes = allowScopes;
      this.ScopeClaimType = scopeClaimType;
    }

    public bool HasPermission(ActionExecutingContext actionExecutingContext)
    {
      actionExecutingContext = actionExecutingContext ?? throw new ArgumentNullException(nameof(actionExecutingContext));
      if (HasMatchingPermissionClaim(PermissionClaimType, actionExecutingContext.HttpContext.User.Claims, x => x))
      {
        return true;
      }
      return AllowScopes && HasMatchingPermissionClaim(ScopeClaimType, actionExecutingContext.HttpContext.User.Claims, x => x.SelectMany(t => t.Split(' ')));
    }

    public bool HasMatchingPermissionClaim(string type, IEnumerable<Claim> claims, Func<IEnumerable<string>, IEnumerable<string>> permissionsSeperator)
    {
      permissionsSeperator = permissionsSeperator ?? throw new ArgumentNullException(nameof(permissionsSeperator));
      var permissions = claims
        .Where(f => type.Equals(f?.Type, StringComparison.CurrentCultureIgnoreCase))
        .Select(t => t.Value)
        .Where(t => !string.IsNullOrWhiteSpace(t));
      if (permissions.Any())
      {
        var userPermissions = permissionsSeperator(permissions);
        return userPermissions.Any(a => AllowedPermissions.Any(t => a.Equals(t, StringComparison.InvariantCultureIgnoreCase)));
      }
      return false;
    }
  }
}
