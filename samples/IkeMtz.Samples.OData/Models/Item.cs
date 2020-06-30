using System;
using System.Collections.Generic;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.OData.Models
{
  public class Item : IIdentifiable, IAuditable
  {
    public Item()
    {
      SubItemAs = new HashSet<SubItemA>();
      SubItemBs = new HashSet<SubItemB>();
      SubItemCs = new HashSet<SubItemC>();
    }
    public Guid Id { get; set; }
    public string Value { get; set; }
    public virtual ICollection<SubItemA> SubItemAs { get; }
    public virtual ICollection<SubItemB> SubItemBs { get; }
    public virtual ICollection<SubItemC> SubItemCs { get; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
  }

  public class SubItemA : IIdentifiable
  {
    public Guid Id { get; set; }
    public string ValueA { get; set; }
    public Guid ItemId { get; set; }
    public virtual Item Item { get; set; }

  }

  public class SubItemB : IIdentifiable
  {

    public Guid Id { get; set; }
    public string ValueB { get; set; }
    public Guid ItemId { get; set; }
    public virtual Item Item { get; set; }
  }

  public class SubItemC : IIdentifiable
  {

    public Guid Id { get; set; }
    public string ValueC { get; set; }
    public Guid ItemId { get; set; }
    public virtual Item Item { get; set; }
  }
}
