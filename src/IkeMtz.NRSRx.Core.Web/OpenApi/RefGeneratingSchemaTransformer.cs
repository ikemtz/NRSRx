using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  /// <summary>
  /// Transforms OpenAPI schemas by generating references for properties marked with a schema id metadata.
  /// Implements <see cref="IOpenApiSchemaTransformer"/> to modify individual schemas and
  /// <see cref="IOpenApiDocumentTransformer"/> to update document-level schema components.
  /// </summary>
  public sealed class RefGeneratingSchemaTransformer : IOpenApiSchemaTransformer, IOpenApiDocumentTransformer
  {
    /// <summary>
    /// Metadata property name used on schema properties to indicate the referenced type name.
    /// The name is formed from the OpenAPI extension prefix and the schema identifier suffix.
    /// </summary>
    public const string SCHEMA_PROPERTY_NAME = $"{OpenApiConstants.ExtensionFieldNamePrefix}{OpenApiConstants.Schema}-id";

    /// <summary>
    /// Monitors <see cref="OpenApiOptions"/> instances so options can be looked up per document.
    /// </summary>
    public readonly IOptionsMonitor<OpenApiOptions> OptionsMonitor;

    /// <summary>
    /// JSON serializer options used to create <see cref="JsonTypeInfo"/> instances when generating schema ids.
    /// </summary>
    public readonly JsonSerializerOptions JsonSerializationOptions;

    /// <summary>
    /// Cache of document-specific <see cref="OpenApiOptions"/> keyed by document name.
    /// </summary>
    public readonly Dictionary<string, OpenApiOptions> DocumentOptions = new();

    /// <summary>
    /// Creates a new instance of <see cref="RefGeneratingSchemaTransformer"/>.
    /// </summary>
    /// <param name="services">The service provider used to resolve required services.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public RefGeneratingSchemaTransformer(IServiceProvider services)
    {
      ArgumentNullException.ThrowIfNull(services);
      OptionsMonitor = services.GetRequiredService<IOptionsMonitor<OpenApiOptions>>();
      JsonSerializationOptions = services.GetService<JsonSerializerOptions>() ??
        new JsonSerializerOptions();
    }

    /// <summary>
    /// Transforms an individual <see cref="OpenApiSchema"/>. If the schema contains properties
    /// with metadata matching <see cref="SCHEMA_PROPERTY_NAME"/>, those properties will be replaced
    /// with a dynamic reference to the corresponding component schema.
    /// </summary>
    /// <param name="schema">The schema to transform.</param>
    /// <param name="context">The transformation context which includes the JSON type information and document name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed <see cref="Task"/>. The transformation is applied in-place on <paramref name="schema"/>.</returns>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
      if (schema == null || context?.JsonTypeInfo == null)
      {
        return Task.CompletedTask;
      }
      DocumentOptions.TryGetValue(context.DocumentName, out var documentOptions);
      if (documentOptions == null)
      {
        documentOptions = OptionsMonitor.Get(context.DocumentName);
        DocumentOptions.Add(context.DocumentName, documentOptions);
      }

      schema.Properties = schema.Properties?.Select(schemaPropKvp =>
      {
        if (schemaPropKvp.Value is OpenApiSchema value && value.Metadata != null && value.Metadata[SCHEMA_PROPERTY_NAME] != null)
        {
          var propertyTypeName = value.Metadata[SCHEMA_PROPERTY_NAME].ToString();
          var contextType = context.JsonTypeInfo.Type;
          if (propertyTypeName.Equals(contextType.Name))
          {
            var referenceId = CreateSchemaReferenceId(documentOptions, contextType);
            return new KeyValuePair<string, IOpenApiSchema>(schemaPropKvp.Key, new OpenApiSchema()
            {
              DynamicRef = $"#/components/schemas/{referenceId}"
            });
          }
        }
        return schemaPropKvp;
      }).ToDictionary();

      return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a schema reference id for the provided <paramref name="referencedType"/> using
    /// the provided <paramref name="openApiOptions"/> and the configured <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="openApiOptions">The OpenAPI options used to generate the schema id.</param>
    /// <param name="referencedType">The .NET type to create a schema reference id for.</param>
    /// <returns>The generated schema reference id or <c>null</c> if one cannot be created.</returns>
    public string? CreateSchemaReferenceId(OpenApiOptions openApiOptions, Type referencedType)
    {
      var referencedJsonTypeInfo = JsonTypeInfo.CreateJsonTypeInfo(referencedType, JsonSerializationOptions);
      var id = openApiOptions.CreateSchemaReferenceId(referencedJsonTypeInfo);
      return id;
    }

    /// <summary>
    /// Transforms the <see cref="OpenApiDocument"/> by converting any schema properties that were
    /// previously marked with a dynamic reference into concrete <see cref="OpenApiSchemaReference"/>
    /// instances that point to the appropriate component schema.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The transformer context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A completed <see cref="Task"/>. The transformation is applied in-place on <paramref name="document"/>.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
      document.Components.Schemas = document.Components.Schemas.Select(schemaKvp =>
      {
        if (schemaKvp.Value.Properties != null && schemaKvp.Value.Properties.Any(a => a.Value.DynamicRef != null))
        {
          var value = schemaKvp.Value as OpenApiSchema;
          var propertiesDictionary = value.Properties.Select(propertyKvp =>
          {
            if (propertyKvp.Value.DynamicRef != null)
            {
              var propertyRefSchema = new OpenApiSchemaReference($"#/components/schemas/{schemaKvp.Key}", document)
              {
                Type = JsonSchemaType.Object,
              };
              return new KeyValuePair<string, IOpenApiSchema>(propertyKvp.Key, propertyRefSchema);
            }
            return propertyKvp;
          }).ToDictionary();
          value.Properties = propertiesDictionary;
          return new KeyValuePair<string, IOpenApiSchema>(schemaKvp.Key, value);
        }
        return schemaKvp;
      }).ToDictionary();
      return Task.CompletedTask;
    }
  }
}
