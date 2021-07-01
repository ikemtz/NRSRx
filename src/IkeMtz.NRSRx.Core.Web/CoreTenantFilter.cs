using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Web
{
  public abstract class CoreTenantFilterAttribute : Attribute, IAuthorizationFilter
  {
    private const string TID = "tid"; 
    protected CoreTenantFilterAttribute()
    { 
    }

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

    public abstract IEnumerable<string> GetUserTenants(HttpContext httpContext);
  }
}
