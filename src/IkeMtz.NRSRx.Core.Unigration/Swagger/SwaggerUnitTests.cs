using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
  public static class SwaggerUnitTests
  {
    /// <summary>
    /// Validates and returns the Swagger page HTML
    /// </summary>
    /// <param name="testServer"></param>
    /// <returns></returns>
    public static async Task<string> TestHtmlPageAsync(TestServer testServer)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      //Get 
      var resp = await client.GetAsync($"index.html").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var html = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);
      var pattern = @"\<title\>Swagger UI\<\/title>";
      var m = Regex.Match(html, pattern);
      Assert.IsTrue(m.Success);
      StringAssert.Contains(html, "<meta name=\"robots\" content=\"none\" />");
      return html;
    }

    /// <summary>
    /// Validates and returns the OpenApiDocument in JSON format
    /// </summary>
    /// <param name="testServer"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static async Task<OpenApiDocument> TestJsonDocAsync(TestServer testServer, int version = 1)
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      var client = testServer.CreateClient();
      //Get 
      var resp = await client.GetAsync($"/swagger/v{version}/swagger.json").ConfigureAwait(true);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);

      // This was required because there's a type mismatch on the OpenApi Doc spec
      var additionPropertiesPattern = @",\s*\""additionalProperties\""\: false";
      result = Regex.Replace(result, additionPropertiesPattern, "");
      var enumPattern = @"""enum"":[\s\[\d,\]]*";
      result = Regex.Replace(result, enumPattern, "");

      var doc = JsonConvert.DeserializeObject<OpenApiDocument>(result, new JsonSerializerSettings() { Error = (x, y) => { } });
      Assert.AreEqual($"{version}.0", doc.Info.Version);
      return doc;
    }
  }
}
