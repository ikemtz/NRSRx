using System;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class DbContextFactory
  {
    public TAuditableDbContext CreateInMemoryAuditableDbContext<TAuditableDbContext>(TestContext testContext)
        where TAuditableDbContext : AuditableDbContext
    {
      var fac = new MockHttpContextAccessorFactory();
      var accessor = fac.CreateAccessor();
      var options = CreateDbContextOptions<TAuditableDbContext>(testContext);
      var constructor = typeof(TAuditableDbContext)
          .GetConstructor(new[] { options.GetType(), accessor.GetType() });
      return (TAuditableDbContext)constructor.Invoke(new object[] { options, accessor });
    }

    public TDbContext CreateInMemoryDbContext<TDbContext>(TestContext testContext)
        where TDbContext : DbContext
    {
      var options = CreateDbContextOptions<TDbContext>(testContext);
      var constructor = typeof(TDbContext)

          .GetConstructor(new[] { options.GetType() });
      return (TDbContext)constructor.Invoke(new object[] { options });
    }

    public DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>(TestContext testContext)
        where TDbContext : DbContext
    {
      return new DbContextOptionsBuilder<TDbContext>()
         .UseInMemoryDatabase(Guid.NewGuid().ToString())
         .EnableSensitiveDataLogging()
         .EnableDetailedErrors()
         .UseLoggerFactory(new LoggerFactory(new[] { new TestContextLoggerProvider(testContext) }))
         .Options;
    }


  }
}
