namespace IkeMtz.NRSRx.Core.Web
{
  public class OAuthScope
  {
    public OAuthScope(string name, string description)
    {
      Name = name;
      Description = description;
    }
    public string Name { get; set; }
    public string Description { get; set; }

    public static OAuthScope OpenId => new("openid", "required");
  }
}
