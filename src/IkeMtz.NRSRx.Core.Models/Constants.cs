using System.Text.Json;
using System.Text.Json.Serialization;

namespace IkeMtz.NRSRx.Core
{
  /// <summary>
  /// Provides application-wide constants.
  /// </summary>
  public static class Constants
  {
    /// <summary>
    /// The default JSON serializer options with camel case property names.
    /// </summary>
    private static JsonSerializerOptions jsonSerializerOptions;
    /// <summary>
    /// Gets the default JSON serializer options.
    /// </summary>
    public static JsonSerializerOptions JsonSerializerOptions
    {
      get
      {
        if (jsonSerializerOptions == null)
        {
          jsonSerializerOptions = new JsonSerializerOptions
          {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            /*
             * This value reflects the max expansion depth of odata queries; adjust as necessary
             */
            MaxDepth = 10,
            AllowTrailingCommas = true,
          };
          jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        return jsonSerializerOptions;
      }
    }
  }
}
