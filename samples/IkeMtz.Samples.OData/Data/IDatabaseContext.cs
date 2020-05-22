using Microsoft.EntityFrameworkCore;
using IkeMtz.Samples.OData.Models;

namespace IkeMtz.Samples.OData.Data
{
  public interface IDatabaseContext
  {
    DbSet<Item> Items { get; set; }
    DbSet<SubItemA> SubItemAs { get; set; }
    DbSet<SubItemB> SubItemBs { get; set; }
    DbSet<SubItemC> SubItemCs { get; set; }
  }
}

