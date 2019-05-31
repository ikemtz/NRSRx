using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IkeMtz.NRSRx.Core.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PermissionsAttribute : Attribute, IActionFilter
    {
        private readonly string permissionClaimType;
        private readonly char permissionClaimSeperator;
        private readonly string[] filters;
        private readonly bool allowScopes;
        private const string scopeClaimType = "scope";

        public PermissionsAttribute(string[] filters, bool allowScopes = true, string permissionClaimType = "permissions", char permissionClaimSeperator = ',')
        {
            this.filters = filters.Select(t => t.ToLower()).ToArray();
            this.permissionClaimSeperator = permissionClaimSeperator;
            this.permissionClaimType = permissionClaimType;
            this.allowScopes = allowScopes;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (HasPermission(permissionClaimType, context.HttpContext.User.Claims, permissionClaimSeperator))
            {
                return;
            }
            else if (allowScopes && HasPermission(scopeClaimType, context.HttpContext.User.Claims, ' '))
            {
                return;
            }
            var errMsg = string.Format($"Permission{0} filter missing or is missing: { string.Join(',', filters)}.", allowScopes ? "/Scope" : "");
            context.Result = new UnauthorizedObjectResult(errMsg);
        }

        public bool HasPermission(string type, IEnumerable<Claim> claims, char sepereator)
        {
            var claim = claims.FirstOrDefault(f => type.Equals(f?.Type, StringComparison.CurrentCultureIgnoreCase));
            if (claim != null)
            {
                var userPermissions = claim.Value.Split(sepereator).Select(t => t.ToLower());
                if (userPermissions.Any(a => filters.Any(t => a == t)))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
