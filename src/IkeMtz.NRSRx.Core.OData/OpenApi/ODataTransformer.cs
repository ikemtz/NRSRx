using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.OData.OpenApi
{
  /// <summary>
  /// Provides transformations for an OpenAPI document to adapt it for OData endpoints.
  /// </summary>
  public class ODataTransformer : IOpenApiDocumentTransformer
  {
    /// <summary>
    /// Transforms the provided <see cref="OpenApiDocument"/> by filtering out internal or
    /// versioned OData paths and by adding OData query string parameters to GET operations.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">Contextual information provided by the OpenAPI generator.</param>
    /// <param name="cancellationToken">Token to observe while performing the transformation.</param>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
      var paths = new Dictionary<string, IOpenApiPathItem>();
      foreach (var path in document.Paths)
      {
        if (!path.Key.EndsWith("$count") && !path.Key.StartsWith("/odata/v{version}/"))
        {
          var operationKvp = path.Value.Operations.FirstOrDefault(t => t.Key == HttpMethod.Get);
          if (operationKvp.Value != null)
          {
            AddODataQueryStringParameters(operationKvp.Value);
          }
          paths.Add(path.Key, path.Value);
        }
      }
      document.Paths.Clear();
      paths.ToList().ForEach(path => document.Paths.Add(path.Key, path.Value));
      return Task.CompletedTask;
    }

    /// <summary>
    /// Adds the standard OData query string parameters (for example, $filter, $orderby, $top, etc.)
    /// to the supplied <see cref="OpenApiOperation"/> so they appear in the generated OpenAPI
    /// specification for GET endpoints.
    /// </summary>
    /// <param name="value">The operation to which OData query parameters will be added.</param>
    public static void AddODataQueryStringParameters(OpenApiOperation value)
    {
      var stringSchema = new OpenApiSchema
      {
        Type = JsonSchemaType.String,
      };
      value.Parameters = [
        new OpenApiParameter
        {
          Name = "$filter",
          In = ParameterLocation.Query,
          Description = "Specifies the logic to pull back a subset of records.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$orderby",
          In = ParameterLocation.Query,
          Description = "Specifies the values used to sort the collection of entries.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$apply",
          In = ParameterLocation.Query,
          Description = "Specifies aggregation behavior for the collection of entries.",
          Schema = stringSchema,
        },new OpenApiParameter
        {
          Name = "$select",
          In = ParameterLocation.Query,
          Description = "The $select system query option allows the clients to requests a limited set of properties for each entry.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$top",
          In = ParameterLocation.Query,
          Description = "Specifies the subset of entries by count.",
          Schema = new OpenApiSchema
          {
            Type = JsonSchemaType.Number,
          },
        },
        new OpenApiParameter
        {
          Name = "$expand",
          In = ParameterLocation.Query,
          Description = "Indicates the related entities to be represented inline. The default maximum depth is 2.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$skip",
          In = ParameterLocation.Query,
          Description = "Specifies the count of entries to skip.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$compute",
          In = ParameterLocation.Query,
          Description = "Specifies computed properties that can be used in $select, $filter or $orderby expressions.",
          Schema = stringSchema,
        },
        new OpenApiParameter
        {
          Name = "$count",
          In = ParameterLocation.Query,
          Description = "Indicates whether or not to include a total entry count.",
          Schema = new OpenApiSchema
          {
            Type = JsonSchemaType.Boolean,
          },
        }];
    }
  }
}
