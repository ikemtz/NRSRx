using System;
using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class DbContextFactory
  {
    public TAuditableDbContext CreateInMemoryAuditableDbContext<TAuditableDbContext>()
        where TAuditableDbContext : AuditableDbContext
    {
      var fac = new MockHttpContextAccessorFactory();
      var accessor = fac.CreateAccessor();
      var options = CreateDbContextOptions<TAuditableDbContext>();
      var constructor = typeof(TAuditableDbContext)
          .GetConstructor(new[] { options.GetType(), accessor.GetType() });
      return (TAuditableDbContext)constructor.Invoke(new object[] { options, accessor });
    }

    public TDbContext CreateInMemoryDbContext<TDbContext>()
        where TDbContext : DbContext
    {
      var options = new DbContextOptionsBuilder<TDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
      var constructor = typeof(TDbContext)
          .GetConstructor(new[] { options.GetType() });
      return (TDbContext)constructor.Invoke(new object[] { options });
    }

    public DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>()
        where TDbContext : DbContext
    {
      return new DbContextOptionsBuilder<TDbContext>()
         .UseInMemoryDatabase(Guid.NewGuid().ToString())
         .Options;
    }


  }
}
