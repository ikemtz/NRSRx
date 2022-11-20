using System;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreMessagingUnigrationTestJob<TProgram> : MessagingJob<TProgram>
        where TProgram : class, IJob
  {
    public TProgram Program { get; }
    public TestContext TestContext { get; }

    public CoreMessagingUnigrationTestJob(TProgram program, TestContext testContext)
    {
      Program = program;
      TestContext = testContext;
    }
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return Program.SetupDependencies(services);
    }

    public override IServiceCollection SetupFunctions(IServiceCollection services)
    {
      return Program.SetupFunctions(services);
    }

    public override void SetupLogging(IServiceCollection services)
    {
      _ = services.AddSingleton(TestContext)
          .AddLogging(x => x.AddTestContext(TestContext));
    }

    public void ExecuteOnContext<TDbContext>(Action<TDbContext> callback) where TDbContext : DbContext
    {
      _ = SetupHost();
      Program.Configuration = this.Configuration;

      callback = callback ?? throw new ArgumentNullException(nameof(callback));

      // Create a scope to obtain a reference to the database
      // context (ApplicationDbContext).
      using var scope = JobHost.Services.CreateAsyncScope();
      var scopedServices = scope.ServiceProvider;
      var logger = scopedServices.GetRequiredService<ILogger<TDbContext>>();
      var db = scopedServices.GetRequiredService<TDbContext>();
      // Ensure the database is created.
      try
      {
        _ = db.Database.EnsureCreated();
      }
      catch (Exception ex)
      {
        TestContext.WriteLine($"DB Creation Exception Occured: {ex}");
      }
      TestContext.WriteLine($"Executing ${nameof(ExecuteOnContext)}<${nameof(TDbContext)}> Logic");
      callback(db);
      if (db is AuditableDbContext)
      {
        var dbContext = db as AuditableDbContext;
        dbContext.CurrentUserProvider = new SystemUserProvider();
        dbContext.SaveChanges(logger);
      }
      else
      {
        _ = db.SaveChanges();
      }
    }
  }
}
