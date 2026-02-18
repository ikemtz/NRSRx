using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Adds common OData query parameters to Swagger documentation for GET operations.
  /// </summary>
  public class ODataCommonOperationFilter : IOperationFilter
  {
    /// <summary>
    /// Applies the filter to the specified operation.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The context for the filter.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      if ((context?.ApiDescription.HttpMethod?.Equals("GET", StringComparison.CurrentCultureIgnoreCase)).GetValueOrDefault())
      {
        AddParameter(operation, "$filter", JsonSchemaType.String, "Specifies the logic to pull back a subset of records.");
        AddParameter(operation, "$orderby", JsonSchemaType.String, "Specifies the values used to sort the collection of entries.");
        AddParameter(operation, "$apply", JsonSchemaType.String, "Specifies aggregation behavior for the collection of entries.");
        AddParameter(operation, "$select", JsonSchemaType.String, "The $select system query option allows the clients to requests a limited set of properties for each entry.");
        AddParameter(operation, "$top", JsonSchemaType.Number, "Specifies the subset of entries by count.");
        AddParameter(operation, "$expand", JsonSchemaType.String, "Indicates the related entities to be represented inline. The default maximum depth is 2.");
        AddParameter(operation, "$skip", JsonSchemaType.Number, "Specifies the count of entries to skip.");
        AddParameter(operation, "$compute", JsonSchemaType.String, "Specifies computed properties that can be used in $select, $filter or $orderby expressions.");
        AddParameter(operation, "$count", JsonSchemaType.Boolean, "Indicates whether or not to include a total entry count.");
      }
    }

    /// <summary>
    /// Adds a parameter to the specified operation if it does not already exist.
    /// </summary>
    /// <param name="operation">The operation to add the parameter to.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The type of the parameter.</param>
    /// <param name="description">The description of the parameter.</param>
    public static void AddParameter([NotNull] OpenApiOperation operation, string name, JsonSchemaType type, string description)
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
