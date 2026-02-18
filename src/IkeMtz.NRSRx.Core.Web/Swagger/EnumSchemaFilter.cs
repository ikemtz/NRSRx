using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  internal class EnumSchemaFilter : ISchemaFilter
  {
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
      if (context.Type.IsEnum)
      {
        schema.Enum.Clear();
        var enumValues = Enum.GetValues(context.Type);
        var attribType = typeof(DefaultValueAttribute);
        var attribs = context.Type.GetCustomAttributesData().Where(w => w.AttributeType == attribType);
        foreach (var i in enumValues)
        {
          var enumVal = Convert.ToInt64(i);
          schema.Enum.Add(JsonValue.Create($"{enumVal} - {i}"));
          //if (attribs.Any())
          //{
          //  var defaultVal = Convert.ToInt64(attribs.First().ConstructorArguments.First().Value);
          //  if (enumVal == defaultVal)
          //  {
          //    schema..Default = JsonValue.Create($"{enumVal} - {i}");
          //  }
          //}
        }
      }
    }
  }
}
