namespace IkeMtz.NRSRx.Core
{
  /// <summary>
  /// Defines a contract for providing the current user ID.
  /// </summary>
  public interface ICurrentUserProvider
  {
    /// <summary>
    /// Gets the current user ID.  Returns null if the request is not authenticated.
    /// </summary>
    /// <param name="defaultValue">The value to return if the user ID is not available.</param>
    /// <returns>The current user ID, or the specified default value if the user ID is not available.</returns>
    public string? GetCurrentUserId(string? defaultValue = null);
  }
}
