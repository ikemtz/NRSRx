using IkeMtz.NRSRx.Core.EntityFramework;
using System.Reflection;
using System;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreJobUnigrationTestProgram<TJob> : Job
        where TJob : Job
  {
    public TJob Program { get; }
    public TestContext TestContext { get; }

    public CoreJobUnigrationTestProgram(TJob program, TestContext testContext)
    {
      Program = program;
      TestContext = testContext;
    }


    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return Program.SetupDependencies(services);
    }

    public override IServiceCollection SetupJobs(IServiceCollection services)
    {
      return Program.SetupJobs(services);
    }
    public override void SetupLogging(IServiceCollection services)
    {
      _ = services.AddSingleton(TestContext)
          .AddLogging(x => x.AddTestContext(TestContext));
    }

    public void ExecuteOnContext<TDbContext>(Action<TDbContext> callback) where TDbContext : DbContext
    {
      _ = SetupHost();

      callback = callback ?? throw new ArgumentNullException(nameof(callback));

      // Create a scope to obtain a reference to the database
      // context (ApplicationDbContext).
      using var scope = JobHost.Services.CreateAsyncScope();
      var scopedServices = scope.ServiceProvider;
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
      if (db is AuditableDbContext)
      {
        var dbType = db.GetType();
        var httpContextAccessor = MockHttpContextAccessorFactory.CreateAccessor();
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        dbType
          .GetProperty("HttpContextAccessor", bindingFlags)
          .SetValue(db, httpContextAccessor);
      }
      TestContext.WriteLine($"Executing ${nameof(ExecuteOnContext)}<${nameof(TDbContext)}> Logic");
      callback(db);
      _ = db.SaveChanges();
    }
  }
}
