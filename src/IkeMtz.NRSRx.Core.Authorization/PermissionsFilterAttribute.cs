using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace IkeMtz.NRSRx.Core.Authorization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class PermissionsFilterAttribute : BaseActionFilterAttribute
  {

    public PermissionsFilterAttribute(string[] allowedPermissions, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
      : base(allowedPermissions, allowScopes, permissionClaimType, permissionClaimSeperator, scopeClaimType)
    {
    }


    public override void OnActionExecuting(ActionExecutingContext context)
    {
      if (!HasPermission(context))
      {
        var errMsg = string.Format($"Permission {0} filter missing or is missing: { string.Join(',', allowedPermissions)}.", allowScopes ? "/Scope" : "");
        context.Result = new UnauthorizedObjectResult(errMsg);
      }
    }
  }
}
