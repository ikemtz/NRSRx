namespace IkeMtz.NRSRx.Core.Web
{
  public class AppSettings
  {
    public string IdentityAudiences { get; set; }
    public string IdentityProvider { get; set; }
    public string InstrumentationKey { get; set; }
    public string DbConnectionString { get; set; }
    public string SwaggerAppName { get; set; }
    public string SwaggerClientId { get; set; }
    public string SwaggerClientSecret { get; set; }
  }
}
