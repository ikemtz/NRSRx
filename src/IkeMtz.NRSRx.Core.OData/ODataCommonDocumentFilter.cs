using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Filters out specific OData schema types and paths from the Swagger documentation.
  /// </summary>
  public class ODataCommonDocumentFilter : IDocumentFilter
  {
    /// <summary>
    /// The list of schema types to be filtered out.
    /// </summary>
    public static readonly string[] FilteredOutSchemaTypes = [
          nameof(IEdmType),
          nameof(IEdmTypeReference),
          nameof(IEdmTerm),
          nameof(IEdmEntityContainer),
          nameof(IEdmModel),
          nameof(IEdmSchemaElement),
          nameof(IEdmDirectValueAnnotationsManager),
          nameof(IEdmVocabularyAnnotatable),
          nameof(IEdmVocabularyAnnotation),
          nameof(IEdmExpression),
          nameof(IEdmEntityContainerElement),

          nameof(EdmContainerElementKind),
          nameof(EdmExpressionKind),
          nameof(EdmSchemaElementKind),
          nameof(EdmTypeKind),

          nameof(ODataEntitySetInfo),
          nameof(ODataFunctionImportInfo),
          nameof(ODataServiceDocument),
          nameof(ODataSingletonInfo),
          nameof(ODataTypeAnnotation),
        ];

    /// <summary>
    /// Applies the filter to the specified Swagger document.
    /// </summary>
    /// <param name="swaggerDoc">The Swagger document to apply the filter to.</param>
    /// <param name="context">The context for the filter.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      foreach (var apiDescription in context.ApiDescriptions)
      {
        if (apiDescription.RelativePath.EndsWith("$count", StringComparison.CurrentCultureIgnoreCase) ||
          "Metadata".Equals((apiDescription.ActionDescriptor as ControllerActionDescriptor)?.ControllerName, StringComparison.CurrentCultureIgnoreCase))
        {
          var route = "/" + apiDescription.RelativePath;
          _ = swaggerDoc.Paths.Remove(route);
        }
      }

      foreach (var schema in FilteredOutSchemaTypes)
      {
        _ = swaggerDoc.Components.Schemas.Remove(schema);
      }
    }
  }
}
