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
    public static async Task<string> TestHtmlPageAsync(TestServer server)
    {
      var client = server.CreateClient();
      //Get 
      var resp = await client.GetAsync($"index.html");

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var html = await resp.Content.ReadAsStringAsync();
      var pattern = @"\<title\>Swagger UI\<\/title>";
      var m = Regex.Match(html, pattern);
      Assert.IsTrue(m.Success);
      return html;
    }

    public static async Task<OpenApiDocument> TestJsonDocAsync(TestServer server, int version = 1)
    {
      var client = server.CreateClient();
      //Get 
      var resp = await client.GetAsync($"v{version}_swagger.json");

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync();

      // This was required because there's a type mismatch on the OpenApi Doc spec
      var pattern = @",\s*\""additionalProperties\""\: false";
      result = Regex.Replace(result, pattern, "");


      var doc = JsonConvert.DeserializeObject<OpenApiDocument>(result, new JsonSerializerSettings() { Error = (x, y) => { } });
      Assert.AreEqual($"{version}.0", doc.Info.Version);
      return doc;
    }
  }
}
