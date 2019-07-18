using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Authorization
{
  public class ExpandPermissionsFilterAttribute : BaseActionFilterAttribute
  {
    private readonly string expandKey = "$expand";
    private readonly string expandClause;

    public ExpandPermissionsFilterAttribute(string[] allowedPermissions, string expandClause, bool allowScopes = true, string permissionClaimType = DefaultPermissionClaimType, char permissionClaimSeperator = ',', string scopeClaimType = DefaultScopeClaimType)
     : base(allowedPermissions, allowScopes, permissionClaimType, permissionClaimSeperator, scopeClaimType)
    {
      this.expandClause = expandClause;
    }


    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
      if (!HasPermission(ctx))
      {
        var collection = ctx.HttpContext.Request.Query.Select(t =>
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
        ctx.HttpContext.Request.Query = new QueryCollection(collection);
      }
    }
  }
}
