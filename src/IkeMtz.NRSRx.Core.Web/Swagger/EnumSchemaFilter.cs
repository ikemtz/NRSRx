using System;
using System.ComponentModel;
using System.Linq;
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
        if (schema is OpenApiSchema openSchema)
        {
          schema.Enum.Clear();
          openSchema.Type = JsonSchemaType.String;
          openSchema.Format = null;
          var enumValues = Enum.GetValues(context.Type);
          var attribType = typeof(DefaultValueAttribute);
          var attribs = context.Type.GetCustomAttributesData().Where(w => w.AttributeType == attribType);
          foreach (var i in enumValues)
          {
            var enumVal = Convert.ToInt64(i);
            openSchema.Enum.Add($"{enumVal} - {i}");
            if (attribs.Any())
            {
              var defaultVal = Convert.ToInt64(attribs.First().ConstructorArguments.First().Value);
              if (enumVal == defaultVal)
              {
                openSchema.Default = $"{enumVal} - {i}";
              }
            }
          }
        }
      }
    }

  }
}
