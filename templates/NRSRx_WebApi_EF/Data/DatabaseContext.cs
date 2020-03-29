using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NRSRx_WebApi_EF.Models;

namespace NRSRx_WebApi_EF.Data
{
  public class DatabaseContext : AuditableDbContext, IDatabaseContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options, httpContextAccessor)
    {
    }

    public virtual DbSet<Value> Values { get; set; }
  }
}
