using Microsoft.EntityFrameworkCore;
using NRSRx_OData_EF.Models;

namespace NRSRx_OData_EF.Data
{
  public class DatabaseContext : DbContext, IDatabaseContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
  }
}
