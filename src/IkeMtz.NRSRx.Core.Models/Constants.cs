using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IkeMtz.NRSRx.Core
{
  public static class Constants
  {
    private readonly static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
      ContractResolver = new CamelCasePropertyNamesContractResolver(),
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };
    public static JsonSerializerSettings JsonSerializerSettings => jsonSerializerSettings;
  }
}
