using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.OData.OpenApi
{
  public class SecurityTransformer : IOpenApiDocumentTransformer
  {
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
      document.Servers.Clear();
      var securityReq = new OpenApiSecurityRequirement
      {
        { new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2)), [nameof(SecuritySchemeType.OAuth2)] }
      };
      document.Security = [securityReq];
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

    public void AddODataQueryStringParameters(OpenApiOperation value)
    {
      var stringSchema = new OpenApiSchema
      {
        Type = JsonSchemaType.String,
      };
      value.Parameters = [new OpenApiParameter
          {
            Name = "$filter",
            In = ParameterLocation.Query,
            Description = "Specifies the logic to pull back a subset of records.",
            Schema = stringSchema,
          },new OpenApiParameter
          {
            Name = "$orderby",
            In = ParameterLocation.Query,
            Description = "Specifies the values used to sort the collection of entries.",
            Schema = stringSchema,
          },new OpenApiParameter
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
          },new OpenApiParameter
          {
            Name = "$top",
            In = ParameterLocation.Query,
            Description = "Specifies the subset of entries by count.",
            Schema = new OpenApiSchema
            {
              Type = JsonSchemaType.Number,
            },
          },new OpenApiParameter
          {
            Name = "$expand",
            In = ParameterLocation.Query,
            Description = "Indicates the related entities to be represented inline. The default maximum depth is 2.",
            Schema = stringSchema,
          },new OpenApiParameter
          {
            Name = "$skip",
            In = ParameterLocation.Query,
            Description = "Specifies the count of entries to skip.",
            Schema = stringSchema,
          },new OpenApiParameter
          {
            Name = "$compute",
            In = ParameterLocation.Query,
            Description = "Specifies computed properties that can be used in $select, $filter or $orderby expressions.",
            Schema = stringSchema,
          },new OpenApiParameter
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
