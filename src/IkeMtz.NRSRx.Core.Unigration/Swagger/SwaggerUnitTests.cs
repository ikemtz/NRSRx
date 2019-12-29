using Microsoft.AspNetCore.TestHost;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
      string pattern = @"\<title\>Swagger UI\<\/title>";
      var m = Regex.Match(html, pattern);
      Assert.IsTrue(m.Success);
      return html;
    }

    public static async Task<OpenApiDocument> TestJsonDocAsync(TestServer server, int version = 1)
    {
      var client = server.CreateClient();
      //Get 
      var resp = await client.GetAsync($"swagger/v{version}/swagger.json");

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      var result = await resp.Content.ReadAsStringAsync();

      var doc = JsonConvert.DeserializeObject<OpenApiDocument>(result);
      Assert.AreEqual($"{version}.0", doc.Info.Version);
      return doc;

    }
  }
}
