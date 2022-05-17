using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  public class ReverseProxyDocumentFilter : IDocumentFilter
  {
    public const string SwaggerReverseProxyBasePath = "swaggerReverseProxyBasePath";
    public IConfiguration Configuration { get; private set; }
    public ReverseProxyDocumentFilter(IConfiguration config)
    {
      Configuration = config;
    }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      var swaggerReverseProxyBasePath = Configuration.GetValue<string>(SwaggerReverseProxyBasePath);
      if (!string.IsNullOrWhiteSpace(swaggerReverseProxyBasePath))
      {
        var paths = swaggerDoc.Paths.ToList();
        for (int i = 0; i < paths.Count; i++)
        {
          var path = paths.ElementAt(i);
          _ = swaggerDoc.Paths.Remove(path.Key);
          swaggerDoc.Paths.Add($"{swaggerReverseProxyBasePath}{path.Key}", path.Value);
        }
      }
    }
  }
}
