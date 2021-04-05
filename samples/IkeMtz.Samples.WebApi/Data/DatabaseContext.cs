using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using IkeMtz.Samples.Models;

namespace IkeMtz.Samples.WebApi.Data
{
  public class DatabaseContext : AuditableDbContext, IDatabaseContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
  }
}
