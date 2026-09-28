using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization.Metadata;
using Microsoft.OpenApi;
using System.Linq;
using System.Text.Json;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  /// <summary>
  /// Transformer that ensures schema nodes are componentized by assigning a stable
  /// schema id to each schema's metadata. This causes the OpenApiSchemaService to
  /// emit $ref references instead of recursively inlining schemas.
  /// </summary>
  public sealed class RefGeneratingSchemaTransformer : IOpenApiSchemaTransformer
  {
    public const string SCHEMA_PROPERTY_NAME = $"{OpenApiConstants.ExtensionFieldNamePrefix}{OpenApiConstants.Schema}-id";
    private readonly IOptionsMonitor<OpenApiOptions> OptionsMonitor;
    private readonly JsonSerializerOptions JsonSerializationOptions;
    private readonly Dictionary<string, OpenApiOptions> DocumentOptions = [];

    public RefGeneratingSchemaTransformer(IServiceProvider services)
    {
      ArgumentNullException.ThrowIfNull(services);
      OptionsMonitor = services.GetRequiredService<IOptionsMonitor<OpenApiOptions>>();
      JsonSerializationOptions = services.GetService<JsonSerializerOptions>() ??
        new JsonSerializerOptions
        {

        };
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
      if (schema == null || context?.JsonTypeInfo == null)
      {
        return Task.CompletedTask;
      }
      DocumentOptions.TryGetValue(context.DocumentName, out var documentOptions);
      if (documentOptions == null)
      {
        DocumentOptions.Add(context.DocumentName, OptionsMonitor.Get(context.DocumentName));
      }

      schema.Properties = schema.Properties?.Select(schemaPropKvp =>
      {
        var value = schemaPropKvp.Value;
        if (value != null && value.Items != null)
        {
          var genericTypeArgumentType = context.JsonTypeInfo.Type.GenericTypeArguments[0];
          var genericTypeArgument = JsonTypeInfo.CreateJsonTypeInfo(genericTypeArgumentType, JsonSerializationOptions);
          //var referenceId = CreateSchemaReferenceId(documentOptions, context.JsonTypeInfo);
          //JsonTypeInfo.CreateJsonTypeInfo()

          return new KeyValuePair<string, IOpenApiSchema>(schemaPropKvp.Key, new OpenApiSchema
          {
            Type = value.Type,
            //Items = new OpenApiSchemaReference
            //{

            //}

          });
        }
        return schemaPropKvp;
      }).ToDictionary();

      return Task.CompletedTask;
    }

    public string? CreateSchemaReferenceId(OpenApiOptions openApiOptions, JsonTypeInfo jsonTypeInfo)
    {

      var id = openApiOptions.CreateSchemaReferenceId(jsonTypeInfo);
      if (!string.IsNullOrEmpty(id))
      {
        return id;
      }

      return null;
    }

  }
}
