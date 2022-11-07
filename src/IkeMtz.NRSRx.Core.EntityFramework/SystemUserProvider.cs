namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class SystemUserProvider : ICurrentUserProvider
  {
    public static readonly string SystemUserId = "NRSRx System User";
    public string? GetCurrentUserId(string? defaultValue = null)
    {
      return defaultValue ?? SystemUserId;
    }
  }
}
