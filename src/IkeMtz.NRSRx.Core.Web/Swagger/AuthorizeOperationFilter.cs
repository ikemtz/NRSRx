using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  /// <summary>
  /// Operation filter to add authorization responses and security requirements to Swagger operations.
  /// </summary>
  public class AuthorizeOperationFilter : IOperationFilter
  {
    /// <summary>
    /// Applies the filter to the specified operation.
    /// </summary>
    /// <param name="operation">The OpenAPI operation.</param>
    /// <param name="context">The operation filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var authAttributes = context?.MethodInfo.DeclaringType.GetCustomAttributes(true)
                      .Union(context.MethodInfo.GetCustomAttributes(true))
                      .OfType<AuthorizeAttribute>();

      if (authAttributes.Any() && operation != null)
      {
        operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(CultureInfo.CurrentCulture), new OpenApiResponse { Description = nameof(HttpStatusCode.Unauthorized) });
        operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(CultureInfo.CurrentCulture), new OpenApiResponse { Description = nameof(HttpStatusCode.Forbidden) });

        var reference = new OpenApiSecuritySchemeReference("OAuth2");
        operation.Security = [new OpenApiSecurityRequirement
        {
          [reference] = ["OAuth2"]
        }];
      }
    }
  }
}
