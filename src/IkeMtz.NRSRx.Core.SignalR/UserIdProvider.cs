using Microsoft.AspNetCore.SignalR;

namespace IkeMtz.NRSRx.Core.SignalR
{
  /// <summary>
  /// Provides a mechanism to retrieve the user ID from a SignalR connection context using a specified claim type.
  /// </summary>
  public class UserIdProvider : IUserIdProvider
  {
    /// <summary>
    /// Gets or sets the claim type used to identify the user ID in SignalR connections.
    /// Default claim type is "sub" (subject).
    /// </summary>
    public static string UserIdClaimType { get; set; } = "sub";
    /// <summary>
    /// Retrieves the user ID from the specified <see cref="HubConnectionContext"/> using the claim type defined in <see cref="UserIdClaimType"/>.
    /// </summary>
    /// <param name="connection">The SignalR hub connection context.</param>
    /// <returns>The user ID as a string if found; otherwise, null.</returns>
    public string? GetUserId(HubConnectionContext connection)
    {
      return connection.User?.FindFirst(UserIdClaimType)?.Value;
    }
  }
}

