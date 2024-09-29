using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// An abstract attribute that provides tenant-based authorization filtering.  For use in multi tenant Apis.
  /// </summary>
  public abstract class CoreTenantFilterAttribute : Attribute, IAuthorizationFilter
  {
    private const string TID = "tid";

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreTenantFilterAttribute"/> class.
    /// </summary>
    protected CoreTenantFilterAttribute()
    {
    }

    /// <summary>
    /// Called to perform authorization.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var httpContext = context.HttpContext;
      if (!httpContext.Request.Query.ContainsKey(TID))
      {
        context.Result = new BadRequestObjectResult($"Query string param {TID} is required for this endpoint.");
        return;
      }

      var tenants = GetUserTenants(httpContext);
      var currentTenant = httpContext.Request.Query[TID];
      if (!tenants.Any())
      {
        context.Result = new UnauthorizedObjectResult($"The current user doesn't have access to any tenants.");
      }
      else if (!tenants.Any(x => x == currentTenant))
      {
        context.Result = new UnauthorizedObjectResult($"The current user doesn't have access to the {currentTenant} tenant.");
      }
    }

    /// <summary>
    /// Gets the tenants associated with the current user.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>A collection of tenant identifiers.</returns>
    public abstract IEnumerable<string> GetUserTenants(HttpContext httpContext);
  }
}
