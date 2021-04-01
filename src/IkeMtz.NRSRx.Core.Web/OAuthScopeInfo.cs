namespace IkeMtz.NRSRx.Core.Web
{
  public class OAuthScopeInfo
  {
    public OAuthScopeInfo(string name, string description)
    {
      Name = name;
      Description = description;
    }
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
