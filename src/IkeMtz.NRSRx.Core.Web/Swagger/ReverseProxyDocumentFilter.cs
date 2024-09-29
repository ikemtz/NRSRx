using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.NRSRx.Core.Web.Swagger
{
  /// <summary>
  /// Document filter to modify Swagger paths for reverse proxy scenarios.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="ReverseProxyDocumentFilter"/> class.
  /// </remarks>
  /// <param name="config">The application configuration.</param>
  public class ReverseProxyDocumentFilter(IConfiguration config) : IDocumentFilter
  {
    /// <summary>
    /// The configuration key for the Swagger reverse proxy base path.
    /// </summary>
    public const string SwaggerReverseProxyBasePath = "swaggerReverseProxyBasePath";

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    public IConfiguration Configuration { get; private set; } = config;

    /// <summary>
    /// Applies the filter to the specified Swagger document.
    /// </summary>
    /// <param name="swaggerDoc">The OpenAPI document.</param>
    /// <param name="context">The document filter context.</param>
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
