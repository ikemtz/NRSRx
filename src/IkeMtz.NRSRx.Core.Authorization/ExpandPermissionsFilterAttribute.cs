using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IkeMtz.NRSRx.Core.Authorization
{
  public sealed class ExpandPermissionsFilterAttribute : BaseActionFilterAttribute
  {
    private readonly string expandKey = "$expand";
    private readonly string expandClause;

    public ExpandPermissionsFilterAttribute(string[] allowedPermissions, string expandClause, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
     : base(allowedPermissions, allowScopes, permissionClaimType, permissionClaimSeperator, scopeClaimType)
    {
      this.expandClause = expandClause;
    }

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
