using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using NRSRx_WebApi_EF.Models;

namespace NRSRx_WebApi_EF.Data
{
  public interface IDatabaseContext : IAuditableDbContext
  {
    DbSet<Value> Values { get; set; }
  }
}

