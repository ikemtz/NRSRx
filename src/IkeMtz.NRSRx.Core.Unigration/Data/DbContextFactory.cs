using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.Data
{
  public static class DbContextFactory
  {
    public static TAuditableDbContext CreateInMemoryAuditableDbContext<TAuditableDbContext>(TestContext testContext)
        where TAuditableDbContext : AuditableDbContext
    {
      var options = CreateDbContextOptions<TAuditableDbContext>(testContext);
      var constructor = typeof(TAuditableDbContext)
          .GetConstructor(new[] { options.GetType(), typeof(SystemUserProvider) });
      return (TAuditableDbContext)constructor.Invoke(new object[] { options, new SystemUserProvider() });
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
      _ = builder.ConfigureTestDbContextOptions(testContext)
        .AddInterceptors(new CalculatableTestInterceptor(),
          new AuditableTestInterceptor(new SystemUserProvider()));
      return builder.Options;
    }

    public static DbContextOptionsBuilder ConfigureTestDbContextOptions(this DbContextOptionsBuilder optionsBuilder, TestContext testContext)
    {
      return optionsBuilder
         .UseInMemoryDatabase($"InMemoryDbForTesting-{testContext.TestName}",
            x => x.EnableNullChecks(true))
         .EnableSensitiveDataLogging()
         .EnableDetailedErrors()
         .LogTo(testContext.WriteLine)
         .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
         .UseLoggerFactory(new LoggerFactory(new[] { new TestContextLoggerProvider(testContext) }));
    }
  }
}
