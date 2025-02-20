namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Provides the specified string as the UserId.
  /// Should only be used in Unit Tests and situations where anonymous authentication is required.
  /// </summary>
  public class UserProvider : ICurrentUserProvider
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProvider"/> class with the specified user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    public UserProvider(string userId)
    {
      UserId = userId;
    }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gets the current user ID.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the user ID is not set.</param>
    /// <returns>The current user ID or the default value if the user ID is not set.</returns>
    public string? GetCurrentUserId(string? defaultValue = null)
    {
      return defaultValue ?? UserId;
    }
  }
}
