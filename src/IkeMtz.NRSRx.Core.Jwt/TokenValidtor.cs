using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IkeMtz.NRSRx.Core.Jwt
{
  public static class TokenValidtor
  {
    public static TokenValidationParameters TokenValidationParameters { get; private set; }
    private static ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    static TokenValidtor() { }
    public static async Task InitAsyc(string metaDataUrl, string issuer, string audience)
    {
      var openIdConfigurationRetriever = new OpenIdConnectConfigurationRetriever();
      _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metaDataUrl, openIdConfigurationRetriever)
      {
        AutomaticRefreshInterval = new TimeSpan(0, 15, 0),
        RefreshInterval = new TimeSpan(0, 15, 0),
      };

      var openIdConnectConfiguration = await _configurationManager.GetConfigurationAsync();
      TokenValidationParameters = new TokenValidationParameters()
      {
        ValidIssuer = issuer,
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

    public static bool ValidateToken(string token)
    {
      _configurationManager.RequestRefresh();
      var handler = new JwtSecurityTokenHandler();
      var principal = Thread.CurrentPrincipal = handler.ValidateToken(token, TokenValidationParameters, out _);
      return principal.Identity.IsAuthenticated;
    }
  }
}
