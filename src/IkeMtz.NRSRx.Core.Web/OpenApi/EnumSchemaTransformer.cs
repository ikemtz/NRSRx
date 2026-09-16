using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  public class EnumSchemaTransformer : IOpenApiSchemaTransformer
  {
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
      if (context?.JsonPropertyInfo?.PropertyType.IsEnum == true)
      {
        schema.Enum ??= [];
        schema.Enum.Clear();
        var enumType = context.JsonPropertyInfo.PropertyType;
        var enumValues = Enum.GetValues(enumType).Cast<object>();
        var convertedEnumValues = enumValues.ToList().Select(enumValue =>
          {
            return new
            {
              name = enumType.GetField(enumValue.ToString()).Name,
              intValue = (int)enumValue,
            };
          });
        var enumUsesCharValues = convertedEnumValues.All(t =>
          26 <= t.intValue &&
          90 >= t.intValue);
        foreach (var item in convertedEnumValues)
        {
          var jsonValue = JsonValue.Create($"{(enumUsesCharValues ? (char)item.intValue : item.intValue)} - {item.name}");
          schema.Enum.Add(jsonValue);
        }
      }
      return Task.CompletedTask;
    }
  }
}
