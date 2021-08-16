using System;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class DbContextFactory
  {
    public static TAuditableDbContext CreateInMemoryAuditableDbContext<TAuditableDbContext>(TestContext testContext)
        where TAuditableDbContext : AuditableDbContext
    {
      var fac = new MockHttpContextAccessorFactory();
      var accessor = fac.CreateAccessor();
      var options = CreateDbContextOptions<TAuditableDbContext>(testContext);
      var constructor = typeof(TAuditableDbContext)
          .GetConstructor(new[] { options.GetType(), accessor.GetType() });
      return (TAuditableDbContext)constructor.Invoke(new object[] { options, accessor });
    }

    public static TDbContext CreateInMemoryDbContext<TDbContext>(TestContext testContext)
        where TDbContext : DbContext
    {
      var options = CreateDbContextOptions<TDbContext>(testContext);
      var constructor = typeof(TDbContext)
          .GetConstructor(new[] { options.GetType() });
      return (TDbContext)constructor.Invoke(new object[] { options });
    }

    public static DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>(TestContext testContext)
        where TDbContext : DbContext
    {
      var builder = new DbContextOptionsBuilder<TDbContext>();
      ConfigureTestDbContextOptions(builder, testContext);
      return builder.Options;
    }

    public static void ConfigureTestDbContextOptions(this DbContextOptionsBuilder optionsBuilder, TestContext testContext)
    {
      optionsBuilder
         .UseInMemoryDatabase($"InMemoryDbForTesting-{testContext.TestName}")
         .EnableSensitiveDataLogging()
         .EnableDetailedErrors()
         .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
         .UseLoggerFactory(new LoggerFactory(new[] { new TestContextLoggerProvider(testContext) }));
    }
  }
}
