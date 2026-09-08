using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
  public class Info
  {
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

  }
}
