namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for objects that can be enabled or disabled.
  /// If used with an IAuditable DbContext, will be automatically be set when record is created and subsequently deleted.
  /// </summary>
  public interface IEnableable
  {
    /// <summary>
    /// Gets or sets a value indicating whether the object is enabled.
    /// </summary>
    bool IsEnabled { get; set; }
  }
}
