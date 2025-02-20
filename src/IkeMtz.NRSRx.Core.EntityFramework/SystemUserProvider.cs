using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Provides a ICurrentUserProvider that is hard coded to provide "NRSRx System User".
  /// </summary>
  public class SystemUserProvider() : UserProvider(SystemUserId)
  {
    public static string? SystemUserId { get; } = "NRSRx System User";
  }
}
