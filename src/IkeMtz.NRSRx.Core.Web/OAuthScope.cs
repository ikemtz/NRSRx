namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Represents an OAuth scope with a name and description.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="OAuthScope"/> class.
  /// </remarks>
  /// <param name="name">The name of the OAuth scope.</param>
  /// <param name="description">The description of the OAuth scope.</param>
  public class OAuthScope(string name, string description)
  {

    /// <summary>
    /// Gets or sets the name of the OAuth scope.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the description of the OAuth scope.
    /// </summary>
    public string Description { get; set; } = description;

    /// <summary>
    /// Gets the predefined OpenId scope.
    /// </summary>
    public static OAuthScope OpenId => new("openid", "required");
  }
}
