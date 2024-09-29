using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.Jwt
{
  /// <summary>
  /// Provides functionality to validate JWT tokens.
  /// </summary>
  public class TokenValidator : ITokenValidator
  {
    /// <summary>
    /// Gets the token validation parameters.
    /// </summary>
    public TokenValidationParameters TokenValidationParameters { get; private set; }

    /// <summary>
    /// Gets the configuration manager for OpenID Connect.
    /// </summary>
    public ConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager { get; private set; }

    /// <summary>
    /// Initializes the token validator with the specified metadata address and audiences.
    /// </summary>
    /// <param name="metaDataAddress">The metadata address.</param>
    /// <param name="audiences">The audience specified in the "aud" claim.  Multiples audiences can be specified by seperating them with a comma. </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InitAsync(string metaDataAddress, string audiences)
    {
      return InitAsync(metaDataAddress, null, audiences);
    }

    /// <summary>
    /// Initializes the token validator with the specified metadata address, issuer, and audiences.
    /// </summary>
    /// <param name="metaDataAddress">The metadata address of the identity server.</param>
    /// <param name="issuer">The value specified in the "iss" (issuer) claim.</param>
    /// <param name="audiences">The audience specified in the "aud" claim.  Multiples audiences can be specified by seperating them with a comma. </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InitAsync(string metaDataAddress, string? issuer, string audiences)
    {
      var openIdConfigurationRetriever = new OpenIdConnectConfigurationRetriever();
      ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metaDataAddress, openIdConfigurationRetriever)
      {
        AutomaticRefreshInterval = new TimeSpan(0, 15, 0),
        RefreshInterval = new TimeSpan(0, 15, 0),
      };

      var openIdConnectConfiguration = await ConfigurationManager.GetConfigurationAsync().ConfigureAwait(false);
      TokenValidationParameters = new TokenValidationParameters()
      {
        ValidIssuer = issuer ?? openIdConnectConfiguration.Issuer,
        ValidAudiences = audiences.Split(','),
        IssuerSigningKeys = openIdConnectConfiguration.SigningKeys,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateTokenReplay = true,
        ValidateActor = true,
      };
    }

    /// <summary>
    /// Validates the specified JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    /// <exception cref="InvalidProgramException">Thrown if the token validator has not been initialized.</exception>
    public bool ValidateToken(string token)
    {
      if (ConfigurationManager == null)
      {
        throw new InvalidProgramException("Token Validator has to be initialized first.");
      }
      ConfigurationManager.RequestRefresh();
      var handler = new JwtSecurityTokenHandler();
      var principal = Thread.CurrentPrincipal = handler.ValidateToken(token, TokenValidationParameters, out _);
      return principal.Identity.IsAuthenticated;
    }
  }
}
