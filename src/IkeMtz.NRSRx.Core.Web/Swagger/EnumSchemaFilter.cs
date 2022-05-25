using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        foreach (int i in Enum.GetValues(context.Type))
        {
          schema.Enum.Add(new OpenApiString($"{i} - {Enum.GetName(context.Type, i)}"));
        }
      }
    }
  }
}
