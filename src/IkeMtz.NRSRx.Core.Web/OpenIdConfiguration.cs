using System;
using System.Linq;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Web
{
  public class OpenIdConfiguration
  {
    [JsonProperty(PropertyName = "authorization_endpoint")] 
    public string AuthorizeEndpoint { get; set; }
    [JsonProperty(PropertyName = "token_endpoint")] 
    public string TokenEndpoint { get; set; }

    public Uri GetAuthorizationEndpointUri(AppSettings appSettings)
    {
      var audience = appSettings?.IdentityAudiences?.Split(',').First();
      return new Uri($"{this.AuthorizeEndpoint}?audience={audience}");
    }
    public Uri GetTokenEndpointUri()
    { 
      return new Uri(TokenEndpoint);
    }
  }
}
