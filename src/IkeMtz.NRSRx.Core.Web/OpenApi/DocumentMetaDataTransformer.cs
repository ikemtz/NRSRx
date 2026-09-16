using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  public class DocumentMetaDataTransformer(string DocumentTitle) : IOpenApiDocumentTransformer
  {
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
      document.Info.Title = DocumentTitle;
      return Task.CompletedTask;
    }
  }
}
