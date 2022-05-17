using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  public class AuthorizeOperationFilter
         : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var authAttributes = context?.MethodInfo.DeclaringType.GetCustomAttributes(true)
                      .Union(context.MethodInfo.GetCustomAttributes(true))
                      .OfType<AuthorizeAttribute>();

      if (authAttributes.Any() && operation != null)
      {
        operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(CultureInfo.CurrentCulture), new OpenApiResponse { Description = nameof(HttpStatusCode.Unauthorized) });
        operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(CultureInfo.CurrentCulture), new OpenApiResponse { Description = nameof(HttpStatusCode.Forbidden) });

        operation.Security = new List<OpenApiSecurityRequirement>();

        var oauth2SecurityScheme = new OpenApiSecurityScheme()
        {
          Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" },
        };


        operation.Security.Add(new OpenApiSecurityRequirement()
        {
          [oauth2SecurityScheme] = new[] { "OAuth2" }
        });
      }
    }
  }
}
