using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.Jwt
{
  /// <summary>
  /// Defines a contract for validating tokens.
  /// </summary>
  public interface ITokenValidator
  {
    /// <summary>
    /// Gets the token validation parameters.
    /// </summary>
    TokenValidationParameters TokenValidationParameters { get; }

    /// <summary>
    /// Initializes the token validator with the specified metadata address and audience.
    /// </summary>
    /// <param name="metaDataAddress">The metadata address.</param>
    /// <param name="audience">The audience.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitAsync(string metaDataAddress, string audience);

    /// <summary>
    /// Initializes the token validator with the specified metadata address, issuer, and audience.
    /// </summary>
    /// <param name="metaDataAddress">The metadata address.</param>
    /// <param name="issuer">The issuer.</param>
    /// <param name="audience">The audience.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitAsync(string metaDataAddress, string issuer, string audience);

    /// <summary>
    /// Validates the specified token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns><c>true</c> if the token is valid; otherwise, <c>false</c>.</returns>
    bool ValidateToken(string token);
  }
}
