using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  /// <summary>
  /// Transformer to set the OpenApiDocument title to the provided DocumentTitle
  /// </summary>
  /// <param name="DocumentTitle"></param>
  public class DocumentMetaDataTransformer(string DocumentTitle) : IOpenApiDocumentTransformer
  {
    /// <summary>
    /// Transforms the OpenApiDocument by setting its title to the provided DocumentTitle.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
      document.Info.Title = DocumentTitle;
      return Task.CompletedTask;
    }
  }
}
