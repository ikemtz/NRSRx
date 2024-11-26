using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Provides methods to save changes to the database in batches asynchronously.
  /// </summary>
  public static class BatchDataSaver
  {
    /// <summary>  
    /// Extension method that saves changes to the database in batches asynchronously without read back.  
    /// Warning: This this method will not set IAuditable properties.
    /// Warning: For performance reasons, be sure to use DbContext Pooling <see href="https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant#dbcontext-pooling"/>.
    /// </summary>  
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>  
    /// <typeparam name="TEntity">The type of the entity.</typeparam>  
    /// <param name="dbContextFactory">The factory function to create a new database context.</param>  
    /// <param name="entities">The collection of entities to save.</param>  
    /// <param name="logger">The logger to use for logging information.</param>  
    /// <param name="batchSize">The size of each batch (default=200).</param>  
    /// <returns>A task that represents the asynchronous operation. The task result contains the total number of records saved.</returns>  
    public static async Task<int> SaveChangesInBatchAsync<TDbContext, TEntity>(Func<TDbContext> dbContextFactory, IEnumerable<TEntity> entities, ILogger? logger = null, int batchSize = 200)
      where TDbContext : AuditableDbContext
      where TEntity : class
    {
      var totalActualRecordsSaved = 0;

      var optimizedChangeTrackerSettings = new ChangeTrackerSettings();

      await SaveChangesInBatchAsync<TEntity>(async (entityName, totalEstimatedRecords) =>
      {
        for (var currentRecorIndex = 0; currentRecorIndex < totalEstimatedRecords; currentRecorIndex += batchSize)
        {
          var dbContext = dbContextFactory();
          optimizedChangeTrackerSettings.ApplySettings(dbContext);
          var batchStartTime = DateTime.UtcNow;
          var batch = entities.Skip(currentRecorIndex).Take(batchSize);
          totalActualRecordsSaved += await ProcessEntityBatchAsync<TDbContext, TEntity>(dbContext, batch);

          logger?.LogInformation("Saved batch of {entityName} records in {elapsedTimeInMs} ms, approximate remaining items {pendingRecordCount}",
                entityName,
                Math.Ceiling(DateTime.UtcNow.Subtract(batchStartTime).TotalMilliseconds),
             Math.Abs(totalEstimatedRecords - (currentRecorIndex + batch.Count())));
        }
        return totalActualRecordsSaved;
      }, entities, logger, batchSize);

      return totalActualRecordsSaved;
    }

    internal static async Task<int> SaveChangesInBatchAsync<TEntity>(Func<string, int, Task<int>> batchLogicAsync, IEnumerable<TEntity> entities, ILogger? logger = null, int batchSize = 200)
      where TEntity : class
    {
      var entityName = typeof(TEntity).Name;
      var totalEstimatedRecords = entities.Count();
      var overallStarttime = DateTime.UtcNow;

      logger?.LogInformation("Saving {totalRecords} {entityName} records via SaveChangesInBatchAsync in batches of {batchSize}",
        totalEstimatedRecords,
        entityName,
        batchSize);

      var totalActualRecordsSaved = await batchLogicAsync(entityName, totalEstimatedRecords);

      logger?.LogInformation("Saved {totalActualRecords} records via SaveChangesInBatchAsync in {elapstedTimeInMs} ms",
        totalActualRecordsSaved,
        Math.Ceiling(DateTime.UtcNow.Subtract(overallStarttime).TotalMilliseconds));

      return totalActualRecordsSaved;
    }

    internal static Task<int> ProcessEntityBatchAsync<TDbContext, TEntity>(TDbContext auditableDbContext, IEnumerable<TEntity> entities, bool efAcceptAllChangesOnSuccess = false)
      where TDbContext : AuditableDbContext
      where TEntity : class
    {
      foreach (var entity in entities)
      {
        _ = auditableDbContext.Add<TEntity>(entity);
      }
      return auditableDbContext.SaveChangesAsync(efAcceptAllChangesOnSuccess);
    }

  }
}
