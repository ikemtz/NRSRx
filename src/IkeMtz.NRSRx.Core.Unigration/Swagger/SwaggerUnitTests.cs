using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
    public static class SwaggerUnitTests
    {
        public static async Task TestHtmlPage(TestServer server)
        {
            var client = server.CreateClient();
            //Get 
            var resp = await client.GetAsync($"swagger/index.html");

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
            var result = await resp.Content.ReadAsStringAsync();
            string pattern = @"\<title\>Swagger UI\<\/title>";
            var m = Regex.Match(result, pattern);
            Assert.IsTrue(m.Success);
        }

        public static async Task<SwaggerDocument> TestJsonDoc(TestServer server, string version = "v1")
        {
            var client = server.CreateClient();
            //Get 
            var resp = await client.GetAsync($"swagger/{version}/swagger.json");

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
            var result = await resp.Content.ReadAsStringAsync();

            var doc = JsonConvert.DeserializeObject<SwaggerDocument>(result);
            Assert.AreEqual("2.0", doc.Swagger);
            Assert.AreEqual("1.0", doc.Info.Version);
            return doc;

        }
    }
}
