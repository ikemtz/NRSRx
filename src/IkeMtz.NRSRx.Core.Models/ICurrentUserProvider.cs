namespace IkeMtz.NRSRx.Core
{
  public interface ICurrentUserProvider
  {
    public string? GetCurrentUserId(string? defaultValue = null);
  }
}
