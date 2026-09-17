using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.OpenApi
{
  /// <summary>
  /// Provides utility methods for testing OpenAPI documentation in unit tests.
  /// </summary>
  public static class OpenApiUnitTests
  {
    /// <summary>
    /// Validates and returns the OpenAPI page HTML.
    /// </summary>
    /// <param name="testServer">The test server instance.</param>
    /// <returns>The HTML content of the OpenAPI page.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the test server is null.</exception>
    public static async Task<string> TestHtmlPageAsync(TestServer testServer)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      // Get the OpenAPI HTML page
      var resp = await client.GetAsync($"index.html").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var html = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);
      var pattern = @"\<title\>.* - Swagger UI\<\/title>";
      var m = Regex.Match(html, pattern);
      Assert.IsTrue(m.Success, "The openapi page doesn't have a valid title.");
      Assert.Contains("<meta name=\"robots\" content=\"none\" />", html);
      return html;
    }

    /// <summary>
    /// Validates and returns the OpenApiDocument in JSON format.
    /// </summary>
    /// <param name="testServer">The test server instance.</param>
    /// <param name="version">The version of the OpenAPI document.</param>
    /// <returns>The OpenApiDocument object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the test server is null.</exception>
    public static async Task<OpenApiDocument> TestJsonDocAsync(TestServer testServer, int version = 1)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      // Get the OpenAPI JSON document
      var resp = await client.GetAsync($"/openapi/v{version}.json").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);

      var readResult = OpenApiDocument.Parse(result);
      var doc = readResult.Document;
      Assert.AreEqual($"{version}.0.0", doc.Info.Version);
      return doc;
    }
  }
}
