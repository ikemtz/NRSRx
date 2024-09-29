using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IkeMtz.NRSRx.Core
{
  /// <summary>
  /// Provides application-wide constants.
  /// </summary>
  public static class Constants
  {
    /// <summary>
    /// The default JSON serializer settings with camel case property names and reference loop handling set to ignore.
    /// </summary>
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
      ContractResolver = new CamelCasePropertyNamesContractResolver(),
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    /// <summary>
    /// Gets the default JSON serializer settings.
    /// </summary>
    public static JsonSerializerSettings JsonSerializerSettings => jsonSerializerSettings;
  }
}
