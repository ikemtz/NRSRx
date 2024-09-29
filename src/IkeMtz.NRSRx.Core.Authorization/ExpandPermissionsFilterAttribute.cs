using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IkeMtz.NRSRx.Core.Authorization
{
  /// <summary>
  /// Attribute to filter actions based on required permissions and modify the expand clause in the query string if permissions are not met.
  /// </summary>
  public sealed class ExpandPermissionsFilterAttribute : BaseActionFilterAttribute
  {
    private readonly string expandKey = "$expand";
    private readonly string expandClause;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandPermissionsFilterAttribute"/> class.
    /// </summary>
    /// <param name="allowedPermissions">The allowed permissions.</param>
    /// <param name="expandClause">The expand clause to be modified if permissions are not met.</param>
    /// <param name="allowScopes">Indicates whether scopes are allowed.</param>
    /// <param name="permissionClaimType">The type of the permission claim.</param>
    /// <param name="permissionClaimSeperator">The separator for permission claims.</param>
    /// <param name="scopeClaimType">The type of the scope claim.</param>
    public ExpandPermissionsFilterAttribute(string[] allowedPermissions, string expandClause, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
     : base(allowedPermissions, allowScopes, permissionClaimType, permissionClaimSeperator, scopeClaimType)
    {
      this.expandClause = expandClause;
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
        var collection = context.HttpContext.Request.Query.Select(t =>
        {
          if (expandKey.Equals(t.Key, StringComparison.InvariantCultureIgnoreCase))
          {
            var val = string.Join(',', t.Value.ToString()
                   .Split(',')
                   .Where(w => !w.Equals(this.expandClause, StringComparison.InvariantCultureIgnoreCase)));
            return new KeyValuePair<string, StringValues>(t.Key, val);
          }
          return t;
        })
          .Where(t => !string.IsNullOrWhiteSpace(t.Value.ToString()))
          .ToDictionary(x => x.Key, x => x.Value);
        context.HttpContext.Request.Query = new QueryCollection(collection);
      }
    }
  }
}
