namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for objects that can calculate their own values.
  /// When used within an IAuditable DbContext, this method is invoked when record is created or updated.
  /// </summary>
  public interface ICalculateable
  {
    /// <summary>
    /// Calculates the values for the object.
    /// </summary>
    void CalculateValues();
  }
}
