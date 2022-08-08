using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  internal class EnumSchemaFilter : ISchemaFilter
  {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
      if (context.Type.IsEnum)
      {
        schema.Enum.Clear();
        schema.Type = "string";
        schema.Format = null;
        var enumValues = Enum.GetValues(context.Type);
        var attribType = typeof(DefaultValueAttribute);
        var attribs = context.Type.GetCustomAttributesData().Where(w => w.AttributeType == attribType);
        foreach (var i in enumValues)
        {
          var enumVal = Convert.ToInt64(i);
          schema.Enum.Add(new OpenApiString($"{enumVal} - {i}"));
          if (attribs.Any())
          {
            var defaultVal = Convert.ToInt64(attribs.First().ConstructorArguments.First().Value);
            if (enumVal == defaultVal)
            {
              schema.Default = new OpenApiString($"{enumVal} - {i}");
            }
          }
        }
      }
    }
  }
}
