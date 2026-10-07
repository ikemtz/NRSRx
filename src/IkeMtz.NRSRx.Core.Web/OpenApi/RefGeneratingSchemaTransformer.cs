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
  /// <seealso href="https://github.com/dotnet/aspnetcore/issues/63857"/>
  public sealed class RefGeneratingSchemaTransformer : IOpenApiSchemaTransformer, IOpenApiDocumentTransformer
  {

    /// <summary>
    /// Prefix used for component schema references inside an OpenAPI document.
    /// This value is the standard path prefix for component schemas (e.g. "#/components/schemas/").
    /// </summary>
    public const string COMPONENT_SCHEMA_PREFIX = "#/components/schemas/";
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
    public readonly Dictionary<string, OpenApiOptions> DocumentOpenApiOptionsDic = [];

    /// <summary>
    /// Cache of schema names that have been generated and that are now referencable.
    /// Schema names follow the "{documentName}.{entityTypeName}" format.
    /// </summary>
    public static readonly HashSet<string> GeneratedReferencableComponents = [];

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
    /// Temporarily replaces self-referencing entity properties with an
    /// <see cref="OpenApiSchema.DynamicRef"/> to avoid recursive schema expansion and
    /// subsequent exceptions during schema generation. The transformer will look up
    /// document-specific OpenAPI options and use schema metadata to discover entity names
    /// before replacing matching properties with dynamic references.
    /// </summary>
    /// <param name="schema">The <see cref="OpenApiSchema"/> to transform.</param>
    /// <param name="context">The <see cref="OpenApiSchemaTransformerContext"/> containing JSON type information and document name.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>A completed <see cref="Task"/>; the transformation is applied in-place on <paramref name="schema"/>.</returns>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
      if (schema == null || context?.JsonTypeInfo == null)
      {
        return Task.CompletedTask;
      }
      DocumentOpenApiOptionsDic.TryGetValue(context.DocumentName, out var documentOpenApiOptions);
      if (documentOpenApiOptions == null)
      {
        documentOpenApiOptions = OptionsMonitor.Get(context.DocumentName);
        DocumentOpenApiOptionsDic.Add(context.DocumentName, documentOpenApiOptions);
      }
      if (schema.Properties?.Count > 0 && schema.Metadata.TryGetValue(SCHEMA_PROPERTY_NAME, out var entityNameObj))
      {
        var entityName = entityNameObj.ToString();
        if (!string.IsNullOrWhiteSpace(entityName))
        {
          schema.Properties = schema.Properties?.Select(schemaPropKvp =>
          {
            return CreatePropertyDynamicRef(context.DocumentName, entityName.ToString(), schemaPropKvp);
          }).ToDictionary();
        }
      }

      return Task.CompletedTask;
    }
    /// <summary>
    /// Inspects a schema property and, when the property's metadata contains a schema id,
    /// either replaces it with an <see cref="OpenApiSchema"/> containing a
    /// <see cref="OpenApiSchema.DynamicRef"/> that points to the appropriate component schema
    /// or recursively inspects nested properties. This method also tracks generated referencable
    /// components to avoid producing duplicate references.
    /// </summary>
    /// <param name="documentName">The name of the OpenAPI document used to qualify component identifiers.</param>
    /// <param name="entityTypeName">The entity type name that represents the parent schema being processed.</param>
    /// <param name="schemaPropKvp">The property key/value pair to inspect and possibly replace.</param>
    /// <returns>The original or modified property key/value pair.</returns>
    public KeyValuePair<string, IOpenApiSchema> CreatePropertyDynamicRef(string documentName, string entityTypeName, KeyValuePair<string, IOpenApiSchema> schemaPropKvp)
    {
      if (schemaPropKvp.Value is OpenApiSchema value && value.Enum == null && value.Metadata != null && !string.IsNullOrEmpty(value.Metadata[SCHEMA_PROPERTY_NAME]?.ToString()))
      {
        var propertyTypeName = value.Metadata[SCHEMA_PROPERTY_NAME].ToString();
        var fullyQualifiedPropertyTypeName = $"{documentName}.{propertyTypeName}";
        var componentSchemaUrl = $"{COMPONENT_SCHEMA_PREFIX}{propertyTypeName}";
        if (GeneratedReferencableComponents.Contains(fullyQualifiedPropertyTypeName))
        {
          return new KeyValuePair<string, IOpenApiSchema>(schemaPropKvp.Key, new OpenApiSchema
          {
            DynamicRef = componentSchemaUrl,
          });
        }
        else if (propertyTypeName.Equals(entityTypeName, StringComparison.CurrentCultureIgnoreCase))
        {
          GeneratedReferencableComponents.Add(fullyQualifiedPropertyTypeName);
          return new KeyValuePair<string, IOpenApiSchema>(schemaPropKvp.Key, new OpenApiSchema
          {
            DynamicRef = componentSchemaUrl
          });
        }
        else
        {
          GeneratedReferencableComponents.Add(fullyQualifiedPropertyTypeName);
        }
        value.Properties = value.Properties?.Select(subSchemaPropKvp =>
        {
          return CreatePropertyDynamicRef(documentName, propertyTypeName, subSchemaPropKvp);
        }).ToDictionary();
      }
      return schemaPropKvp;
    }

    /// <summary>
    /// Transforms the <see cref="OpenApiDocument"/> by converting any schema properties that were
    /// previously marked with a <see cref="OpenApiSchema.DynamicRef"/> into concrete <see cref="OpenApiSchemaReference"/>
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
              var propertyRefSchema = new OpenApiSchemaReference(propertyKvp.Value.DynamicRef, document)
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
