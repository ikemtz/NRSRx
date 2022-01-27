using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  public class ODataCommonOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if ((context?.ApiDescription.HttpMethod?.Equals("GET", StringComparison.CurrentCultureIgnoreCase)).GetValueOrDefault())
      {
        AddParameter(operation, "$filter", "string", "Specifies the logic to pull back a subset of records.");
        AddParameter(operation, "$orderby", "string", "Specifies the values used to sort the collection of entries.");
        AddParameter(operation, "$apply", "string", "Specifies aggregation behavior for the collection of entries.");
        AddParameter(operation, "$top", "number", "Specifies the subset of entries by count.");
        AddParameter(operation, "$expand", "string", "Indicates the related entities to be represented inline.  The default maximum depth is 2.");
        AddParameter(operation, "$skip", "number", "Specifies the count of entries to skip.");
        AddParameter(operation, "$count", "boolean", "Indicates whether or not to include a total entry count.");
        AddParameter(operation, "$compute", "string", "Specifies computed properties that can be used in a $select, $filter or $orderby expressions.");
      }
    }

    public static void AddParameter([NotNull] OpenApiOperation operation, string name, string type, string description)
    {
      if (operation != null && !operation.Parameters.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
      {
        var parameter = new OpenApiParameter()
        {
          Name = name,
          In = ParameterLocation.Query,
          Description = description,
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
