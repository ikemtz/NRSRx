namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for objects that are associated with a tenant.
  /// Used in multi-tenant applications.
  /// </summary>
  public interface ITentantable
  {
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public string TenantId { get; set; }
  }
}
