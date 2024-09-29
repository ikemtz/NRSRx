using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Authorization
{
  /// <summary>
  /// Attribute to filter actions based on required permissions.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class PermissionsFilterAttribute : BaseActionFilterAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionsFilterAttribute"/> class.
    /// </summary>
    /// <param name="allowedPermissions">The allowed permissions.</param>
    /// <param name="allowScopes">Indicates whether scopes are allowed.</param>
    /// <param name="permissionClaimType">The type of the permission claim.</param>
    /// <param name="permissionClaimSeperator">The separator for permission claims.</param>
    /// <param name="scopeClaimType">The type of the scope claim.</param>
    public PermissionsFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
      : base(allowedPermissions, allowScopes, permissionClaimType, permissionClaimSeperator, scopeClaimType)
    {
    }

    /// <summary>
    /// Called before the action method is executed.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      context = context ?? throw new ArgumentNullException(nameof(context));

      if (!HasPermission(context))
      {
        var errMsg = string.Format(CultureInfo.CurrentCulture, "You do not have one of the required permissions {0}.", string.Join(',', AllowedPermissions));
        context.Result = new UnauthorizedObjectResult(errMsg);
      }
    }
  }
}
