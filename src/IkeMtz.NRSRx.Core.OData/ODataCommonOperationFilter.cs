using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  public class ODataCommonOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      AddParameter(operation, "$filter");
      AddParameter(operation, "$orderby");
      AddParameter(operation, "$top", "number");
      AddParameter(operation, "$skip", "number");
      AddParameter(operation, "$count", "boolean");
    }

    public static void AddParameter(OpenApiOperation operation, string name, string type = "string")
    {
      if (!operation.Parameters.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
      {
        var parameter = new OpenApiParameter()
        {
          Name = name,
          In = ParameterLocation.Query,
          Description = $"OData {name} parameter",
          Required = false,
          Schema = new OpenApiSchema()
          {
            Type = type
          }
        };
        operation.Parameters.Add(parameter);
      }
    }
  }
}
