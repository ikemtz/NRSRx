using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.WebApi
{
  /// <summary>
  /// Represents the result of a ping request.
  /// </summary>
  public class PingResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PingResult"/> class.
    /// </summary>
    /// <param name="apiVersion">The API version.</param>
    public PingResult(ApiVersion apiVersion)
    {
      if (apiVersion != null)
      {
        Version = $"v{apiVersion.MajorVersion}.{apiVersion.MinorVersion ?? 0}";
      }
      else
      {
        Version = "Unknown";
      }
    }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the version of the API.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets or sets the build number of the service.
    /// </summary>
    public string Build { get; set; }
  }
}
