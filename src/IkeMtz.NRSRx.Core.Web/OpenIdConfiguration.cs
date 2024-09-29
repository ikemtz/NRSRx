using System;
using System.Linq;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Represents the OpenID configuration with endpoints for authorization and token.
  /// </summary>
  public class OpenIdConfiguration
  {
    /// <summary>
    /// Gets or sets the authorization endpoint.
    /// </summary>
    [JsonProperty(PropertyName = "authorization_endpoint")]
    public string AuthorizeEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the token endpoint.
    /// </summary>
    [JsonProperty(PropertyName = "token_endpoint")]
    public string TokenEndpoint { get; set; }

    /// <summary>
    /// Gets the authorization endpoint URI with the specified audience from the application settings.
    /// </summary>
    /// <param name="appSettings">The application settings.</param>
    /// <returns>The authorization endpoint URI.</returns>
    public Uri GetAuthorizationEndpointUri(AppSettings appSettings)
    {
      var audience = appSettings?.IdentityAudiences?.Split(',').First();
      return new Uri($"{this.AuthorizeEndpoint}?audience={audience}");
    }

    /// <summary>
    /// Gets the token endpoint URI.
    /// </summary>
    /// <returns>The token endpoint URI.</returns>
    public Uri GetTokenEndpointUri()
    {
      return new Uri(TokenEndpoint);
    }
  }
}
