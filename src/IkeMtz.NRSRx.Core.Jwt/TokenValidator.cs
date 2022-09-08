using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.Jwt
{
  public class TokenValidator : ITokenValidator
  {
    public TokenValidationParameters TokenValidationParameters { get; private set; }
    public ConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager { get; private set; }
    public Task InitAsync(string metaDataAddress, string audience)
    {
      return InitAsync(metaDataAddress, null, audience);
    }

    public async Task InitAsync(string metaDataAddress, string? issuer, string audience)
    {
      var openIdConfigurationRetriever = new OpenIdConnectConfigurationRetriever() { };
      ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metaDataAddress, openIdConfigurationRetriever)
      {
        AutomaticRefreshInterval = new TimeSpan(0, 15, 0),
        RefreshInterval = new TimeSpan(0, 15, 0),
      };

      var openIdConnectConfiguration = await ConfigurationManager.GetConfigurationAsync().ConfigureAwait(false);
      TokenValidationParameters = new TokenValidationParameters()
      {
        ValidIssuer = issuer ?? openIdConnectConfiguration.Issuer,
        ValidAudience = audience,
        IssuerSigningKeys = openIdConnectConfiguration.SigningKeys,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateTokenReplay = true,
        ValidateActor = true,
      };
    }

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
