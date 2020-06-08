using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
  /// </summary>
  /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
  /// Once they are fixed and published, this class can be removed.</remarks>
  public class SwaggerDefaultValues : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var apiDescription = context.ApiDescription;
      operation.Deprecated = apiDescription.IsDeprecated();
      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
      foreach (var parameter in operation.Parameters)
      {
        var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
        if (description == null)
        {
          continue;
        }
        else if (parameter.Description == null)
        {
          parameter.Description = description.ModelMetadata?.Description;
        }
        parameter.Required |= description.IsRequired;
      }
    }
  }
}
