using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  internal class ChangeTrackerSettings
  {
    /// <summary>
    /// This will default to the following write optimized settings:
    /// AutoDetectChangesEnabled = false,
    /// QueryTrackingBehavior = QueryTrackingBehavior.NoTracking,
    /// LazyLoadingEnabled = false
    /// </summary>
    public ChangeTrackerSettings() { }
    public ChangeTrackerSettings(DbContext dbContext)
    {
      AutoDetectChangesEnabled = dbContext.ChangeTracker.AutoDetectChangesEnabled;
      QueryTrackingBehavior = dbContext.ChangeTracker.QueryTrackingBehavior;
      LazyLoadingEnabled = dbContext.ChangeTracker.LazyLoadingEnabled;
    }

    public bool AutoDetectChangesEnabled { get; } = false;
    public QueryTrackingBehavior QueryTrackingBehavior { get; } = QueryTrackingBehavior.NoTracking;
    public bool LazyLoadingEnabled { get; } = false;

    public void ApplySettings(DbContext dbContext)
    {
      dbContext.ChangeTracker.AutoDetectChangesEnabled = AutoDetectChangesEnabled;
      dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior;
      dbContext.ChangeTracker.LazyLoadingEnabled = LazyLoadingEnabled;
    }
  }
}
