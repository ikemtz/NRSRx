using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Web
{
  public abstract class CoreTenantFilterAttribute : Attribute, IAuthorizationFilter
  {
    private const string TID = "tid";
    private readonly Func<IEnumerable<Claim>, IEnumerable<string>> _tenantIdentificationLogic;
    protected CoreTenantFilterAttribute(Func<IEnumerable<Claim>, IEnumerable<string>> TenantIdentificationLogic)
    {
      _tenantIdentificationLogic = TenantIdentificationLogic;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var httpContext = context.HttpContext;
      if (!httpContext.Request.Query.ContainsKey(TID))
      {
        context.Result = new BadRequestObjectResult($"Query string param {TID} is required for this endpoint.");
        return;
      }

      var tenants = _tenantIdentificationLogic(httpContext.User.Claims);
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
  }
}
