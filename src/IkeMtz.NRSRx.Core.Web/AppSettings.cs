namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Represents the application settings.
  /// </summary>
  public class AppSettings
  {
    /// <summary>
    /// Gets or sets the identity audiences.  Multiples can be specified by seperating them with a comma.
    /// </summary>
    public string IdentityAudiences { get; set; }

    /// <summary>
    /// Gets or sets the identity provider.
    /// </summary>
    public string IdentityProvider { get; set; }

    /// <summary>
    /// Gets or sets the database connection string.
    /// </summary>
    public string DbConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the Swagger application name.
    /// </summary>
    public string SwaggerAppName { get; set; }

    /// <summary>
    /// Gets or sets the Swagger client ID.
    /// </summary>
    public string SwaggerClientId { get; set; }

    /// <summary>
    /// Gets or sets the Swagger client secret.
    /// </summary>
    public string SwaggerClientSecret { get; set; }
  }
}
