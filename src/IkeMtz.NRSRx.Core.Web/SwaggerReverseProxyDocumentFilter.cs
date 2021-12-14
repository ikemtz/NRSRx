using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web
{
  public class SwaggerReverseProxyDocumentFilter : IDocumentFilter
  {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      var paths = swaggerDoc.Paths.ToList();
      for (int i = 0; i < paths.Count; i++)
      {
        var path = paths.ElementAt(i);
        _ = swaggerDoc.Paths.Remove(path.Key);
        swaggerDoc.Paths.Add($"/.{path.Key}", path.Value);
      }
    }
  }
}
