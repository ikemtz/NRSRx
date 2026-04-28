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
        return jsonSerializerOptions ??= ConfigureJsonSerializerOptions();
      }
    }

    /// <summary>
    /// Configures a <see cref="JsonSerializerOptions"/> instance with the application's standard defaults.
    /// </summary>
    /// <param name="options">
    /// An optional <see cref="JsonSerializerOptions"/> instance to configure. If <c>null</c>, a new
    /// instance will be created and assigned to the internal default field.
    /// </param>
    /// <returns>The configured <see cref="JsonSerializerOptions"/> instance.</returns>
    public static JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions? options = null)
    {
      options ??= new JsonSerializerOptions();
      options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      options.WriteIndented = true;
      options.MaxDepth = 10;
      options.AllowTrailingCommas = true;
      options.ReferenceHandler = ReferenceHandler.Preserve;
      options.Converters.Add(new JsonStringEnumConverter());
      return options;
    }
  }
}
