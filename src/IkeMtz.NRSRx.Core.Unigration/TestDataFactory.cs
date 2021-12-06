using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class TestDataFactory
  {
    public static TENTITY IdentifiableFactory<TENTITY>(TENTITY value = null)
      where TENTITY : class, IIdentifiable, new()
    {
      if (value == null)
      {
        value = new TENTITY();
      }
      value.Id = Guid.NewGuid();
      return value;
    }

    public static TENTITY AuditableFactory<TENTITY>(TENTITY value = null)
      where TENTITY : class, IAuditable, new()
    {
      if (value == null)
      {
        value = new TENTITY();
      }
      value.CreatedBy = "Auditable Factory";
      value.CreatedOnUtc = DateTime.UtcNow;
      return value;
    }
  }
}
