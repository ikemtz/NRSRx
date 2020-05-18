using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.WebApi
{
  public class PingResult
  {
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
    public string Name { get; set; }
    public string Version { get; }
    public string Build { get; set; }
  }
}
