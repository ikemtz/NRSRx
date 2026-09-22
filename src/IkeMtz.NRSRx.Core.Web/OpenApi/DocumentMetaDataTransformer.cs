using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace IkeMtz.NRSRx.Core.Web.OpenApi
{
  /// <summary>
  /// Transformer to set the OpenApiDocument title to the provided DocumentTitle
  /// </summary> 
  public class DocumentMetaDataTransformer(CoreWebStartup startup) : IOpenApiDocumentTransformer
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
      var buildNumber = startup.GetBuildNumber();
      document.Info.Title = startup.ServiceTitle;
      document.Info.Description = $"<div style='color:gray;font-weight:bold'>Build #: <span style='font-weight:bolder'>{buildNumber}</span></div>";
      document.Info.Version = buildNumber;
      return Task.CompletedTask;
    }
  }
}
