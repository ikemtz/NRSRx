using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
    public class SwaggerDocument
    {
        [JsonProperty("swagger")]
        public string Swagger { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }
    public partial class Info
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
