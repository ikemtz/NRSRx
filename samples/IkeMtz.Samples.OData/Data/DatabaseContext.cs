using Microsoft.EntityFrameworkCore;
using IkeMtz.Samples.OData.Models;

namespace IkeMtz.Samples.OData.Data
{
  public class DatabaseContext : DbContext, IDatabaseContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<SubItemA> SubItemAs { get; set; }
    public virtual DbSet<SubItemB> SubItemBs { get; set; }
    public virtual DbSet<SubItemC> SubItemCs { get; set; }
  }
}
