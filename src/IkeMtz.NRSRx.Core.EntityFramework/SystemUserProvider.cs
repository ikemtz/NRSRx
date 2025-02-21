using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Provides a ICurrentUserProvider that is hard coded to provide "NRSRx System User".
  /// </summary>
  public class SystemUserProvider() : UserProvider(SystemUserId)
  {
    /// <summary>
    /// Gets or sets the system user ID.
    /// </summary>
    public static string? SystemUserId { get; set; } = "NRSRx System User";
  }
}
