namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Provides the system user ID as the current user ID.
  /// </summary>
  public class SystemUserProvider : ICurrentUserProvider
  {
    /// <summary>
    /// Gets or sets the system user ID.
    /// </summary>
    public static string SystemUserId { get; set; } = "NRSRx System User";

    /// <summary>
    /// Gets the current user ID. Returns the system user ID if no default value is provided.
    /// </summary>
    /// <param name="defaultValue">The value to return if the user ID is not available.</param>
    /// <returns>The current user ID, or the specified default value if the user ID is not available.</returns>
    public string? GetCurrentUserId(string? defaultValue = null)
    {
      return defaultValue ?? SystemUserId;
    }
  }
}
