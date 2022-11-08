namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class SystemUserProvider : ICurrentUserProvider
  {
    public static string SystemUserId { get; set; } = "NRSRx System User";
    public string? GetCurrentUserId(string? defaultValue = null)
    {
      return defaultValue ?? SystemUserId;
    }
  }
}
