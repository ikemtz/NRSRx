using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization.Metadata;
using Microsoft.OpenApi;

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
    private readonly IOptionsMonitor<OpenApiOptions>? _optionsMonitor;

    public RefGeneratingSchemaTransformer(IServiceProvider services)
    {
      ArgumentNullException.ThrowIfNull(services);
      _optionsMonitor = services.GetService<IOptionsMonitor<OpenApiOptions>>();
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
      if (schema == null || context?.JsonTypeInfo == null)
      {
        return Task.CompletedTask;
      }

      var jsonTypeInfo = context.JsonTypeInfo;
      var schemaId = CreateSchemaReferenceId(context.DocumentName, jsonTypeInfo);

      // If this is the concrete OpenApiSchema type we can set Metadata directly.
      if (schema is OpenApiSchema openApiSchema)
      {
        openApiSchema.Metadata ??= new Dictionary<string, object?>();
        openApiSchema.Metadata[SCHEMA_PROPERTY_NAME] = schemaId;
      }
      else
      {
        // Best-effort: try to set a Metadata property via reflection without throwing.
        try
        {
          var metadataProp = schema.GetType().GetProperty("Metadata");
          if (metadataProp != null)
          {
            if (metadataProp.GetValue(schema) is IDictionary<string, object?> existing)
            {
              existing[SCHEMA_PROPERTY_NAME] = schemaId;
            }
            else
            {
              var dict = new Dictionary<string, object?>
              {
                [SCHEMA_PROPERTY_NAME] = schemaId
              };
              metadataProp.SetValue(schema, dict);
            }
          }
        }
        catch
        {
          // Ignore - transformer must not fail the generation pipeline.
        }
      }

      return Task.CompletedTask;
    }

    private string CreateSchemaReferenceId(string documentName, JsonTypeInfo jsonTypeInfo)
    {
      try
      {
        if (_optionsMonitor != null && !string.IsNullOrEmpty(documentName))
        {
          var opts = _optionsMonitor.Get(documentName);
          if (opts.CreateSchemaReferenceId != null)
          {
            var id = opts.CreateSchemaReferenceId(jsonTypeInfo);
            if (!string.IsNullOrEmpty(id))
            {
              return id;
            }
          }
        }
      }
      catch
      {
        // swallow and fall back to deterministic id
      }

      var type = jsonTypeInfo.Type;
      var fullName = type.FullName ?? type.Name;
      var sanitized = fullName.Replace('.', '_').Replace('+', '_').Replace('`', '_');
      return sanitized;
    }

  }
}
