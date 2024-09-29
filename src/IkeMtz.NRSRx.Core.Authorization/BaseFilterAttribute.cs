using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Authorization
{
  /// <summary>
  /// Base class for action filter attributes that handle permission checks.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="BaseActionFilterAttribute"/> class.
  /// </remarks>
  /// <param name="allowedPermissions">The allowed permissions.</param>
  /// <param name="allowScopes">Indicates whether scopes are allowed.</param>
  /// <param name="permissionClaimType">The type of the permission claim.</param>
  /// <param name="permissionClaimSeperator">The separator for permission claims.</param>
  /// <param name="scopeClaimType">The type of the scope claim.</param>
  public abstract class BaseActionFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = BaseActionFilterAttribute.DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = BaseActionFilterAttribute.DefaultScopeClaimType) : ActionFilterAttribute
  {
    public const string DefaultPermissionClaimType = "permissions";
    public const string DefaultScopeClaimType = "scope";
    public string[] AllowedPermissions { get; private set; } = allowedPermissions;
    public string PermissionClaimType { get; private set; } = permissionClaimType;
    public char PermissionClaimSeperator { get; private set; } = permissionClaimSeperator;
    public bool AllowScopes { get; private set; } = allowScopes;
    public string ScopeClaimType { get; private set; } = scopeClaimType;

    /// <summary>
    /// Checks if the current user has the required permissions.
    /// </summary>
    /// <param name="actionExecutingContext">The action executing context.</param>
    /// <returns>True if the user has the required permissions; otherwise, false.</returns>
    public bool HasPermission(ActionExecutingContext actionExecutingContext)
    {
      actionExecutingContext = actionExecutingContext ?? throw new ArgumentNullException(nameof(actionExecutingContext));
      if (HasMatchingPermissionClaim(PermissionClaimType, actionExecutingContext.HttpContext.User.Claims, x => x))
      {
        return true;
      }
      return AllowScopes && HasMatchingPermissionClaim(ScopeClaimType, actionExecutingContext.HttpContext.User.Claims, x => x.SelectMany(t => t.Split(' ')));
    }

    /// <summary>
    /// Checks if the user has a matching permission claim.
    /// </summary>
    /// <param name="type">The type of the claim.</param>
    /// <param name="claims">The user's claims.</param>
    /// <param name="permissionsSeperator">Function to separate permissions.</param>
    /// <returns>True if a matching permission claim is found; otherwise, false.</returns>
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
