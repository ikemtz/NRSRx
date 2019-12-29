using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
  /// </summary>
  /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
  /// Once they are fixed and published, this class can be removed.</remarks>
  public class SwaggerDefaultValues : IOperationFilter
  {
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
      Justification = "The OpenApiOperation and OperationFilterContext will be provided by the Swagger libraries")]
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var apiDescription = context.ApiDescription;

      operation.Deprecated = apiDescription.IsDeprecated();

      if (operation.Parameters == null)
      {
        return;
      }

      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
      // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
      foreach (var parameter in operation.Parameters)
      {
        var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

        if (parameter.Description == null)
        {
          parameter.Description = description.ModelMetadata?.Description;
        }
        parameter.Required |= description.IsRequired;
      }
    }
  }
}
