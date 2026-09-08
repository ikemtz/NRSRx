using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Swagger
{
  public class OpenApiDocument
  {
    [JsonProperty("info")]
    public Info Info { get; set; }

    public Dictionary<string, Object> Paths { get; set; } = new Dictionary<string, Object>();
  }
}
