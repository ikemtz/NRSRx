using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
  /// <summary>
  /// Provides utility methods for testing Swagger documentation in unit tests.
  /// </summary>
  public static class SwaggerUnitTests
  {
    /// <summary>
    /// Validates and returns the Swagger page HTML.
    /// </summary>
    /// <param name="testServer">The test server instance.</param>
    /// <returns>The HTML content of the Swagger page.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the test server is null.</exception>
    public static async Task<string> TestHtmlPageAsync(TestServer testServer)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      // Get the Swagger HTML page
      var resp = await client.GetAsync($"index.html").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var html = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);
      var pattern = @"\<title\>.* - Swagger UI\<\/title>";
      var m = Regex.Match(html, pattern);
      Assert.IsTrue(m.Success, "The swagger page doesn't have a valid title.");
      StringAssert.Contains(html, "<meta name=\"robots\" content=\"none\" />");
      return html;
    }

    /// <summary>
    /// Validates and returns the OpenApiDocument in JSON format.
    /// </summary>
    /// <param name="testServer">The test server instance.</param>
    /// <param name="version">The version of the Swagger document.</param>
    /// <returns>The OpenApiDocument object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the test server is null.</exception>
    public static async Task<OpenApiDocument> TestJsonDocAsync(TestServer testServer, int version = 1)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      // Get the Swagger JSON document
      var resp = await client.GetAsync($"/swagger/v{version}/swagger.json").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);

      result = FixSwaggerDocument(result);

      var doc = JsonConvert.DeserializeObject<OpenApiDocument>(result, new JsonSerializerSettings() { Error = (x, y) => { } });
      Assert.AreEqual($"{version}.0", doc.Info.Version);
      return doc;
    }

    /// <summary>
    /// Validates the support for reverse proxy in the OpenApiDocument in JSON format.
    /// </summary>
    /// <param name="testServer">The test server instance.</param>
    /// <param name="swaggerReverseProxyDocumentFilter">The reverse proxy document filter.</param>
    /// <param name="version">The version of the Swagger document.</param>
    /// <returns>The OpenApiDocument object.</returns>
    /// <exception cref="InvalidProgramException">Thrown when the reverse proxy document filter is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the test server is null.</exception>
    public static async Task<OpenApiDocument> TestReverseProxyJsonDocAsync(TestServer testServer, string swaggerReverseProxyDocumentFilter = "", int version = 1)
    {
      if (string.IsNullOrWhiteSpace(swaggerReverseProxyDocumentFilter))
      {
        throw new InvalidProgramException($"{nameof(swaggerReverseProxyDocumentFilter)} should not be empty.");
      }
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      // Get the Swagger JSON document
      var resp = await client.GetAsync($"/swagger/v{version}/swagger.json").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);

      result = FixSwaggerDocument(result);

      var doc = JsonConvert.DeserializeObject<OpenApiDocument>(result, new JsonSerializerSettings() { Error = (x, y) => { } });

      var paths = doc.Paths.Select(t => t.Key).ToList();
      paths.ForEach(x => StringAssert.StartsWith(x, swaggerReverseProxyDocumentFilter, $"Path {x} does not start with the desired '/./'"));
      return doc;
    }

    /// <summary>
    /// Fixes known issues in the Swagger document JSON string.
    /// </summary>
    /// <param name="result">The Swagger document JSON string.</param>
    /// <returns>The fixed Swagger document JSON string.</returns>
    public static string FixSwaggerDocument(string result)
    {
      var additionPropertiesPattern = @",\s*\""additionalProperties\""\: false";
      result = Regex.Replace(result, additionPropertiesPattern, "");
      var defaultZeroPattern = @"""default"": ""?[\d \w-]*""?,?$";
      result = Regex.Replace(result, defaultZeroPattern, "", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
      var enumPattern = @"\""enum\"": \[[\s\""0-9A-z-,]*],";
      result = Regex.Replace(result, enumPattern, "");
      return result;
    }
  }
}
