using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.OData
{
  public class ODataCommonDocumentFilter : IDocumentFilter
  {
    public static readonly string[] FilteredOutSchemaTypes = new[] {
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
    };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      foreach (var apiDescription in context.ApiDescriptions)
      {
        if (apiDescription.RelativePath.EndsWith("$count", StringComparison.CurrentCultureIgnoreCase ) ||
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
