using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{

  /// <summary>  
  /// Provides batch operation extension methods for <see cref="DbContext"/>.  
  /// </summary>  
  public partial class AuditableDbContext
  {

    /// <summary>  
    /// Extension method that saves changes to the database in batches asynchronously without read back.  
    /// Warning: This this method will not set IAuditable properties.
    /// Warning: For performance reasons, be sure to use DbContext Pooling <see href="https://learn.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-di%2Cexpression-api-with-constant#dbcontext-pooling"/>.
    /// </summary>  
    /// <typeparam name="TEntity">The type of the entity.</typeparam>  
    /// <param name="dbContext">The database context.</param>  
    /// <param name="entities">The collection of entities to save.</param>  
    /// <param name="batchSize">The size of each batch (default=200).</param>  
    /// <param name="logger">The logger to use for logging information.</param>
    /// <param name="efAcceptAllChangesOnSuccess">Indicates whether <see cref="DbContext.ChangeTracker.AcceptAllChanges"/> is called after the changes are saved to the database.  Setting this value to true has a performance cost, but may be necessary if not using UniqueIdentier ids</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total number of records saved.</returns>  
    public async Task<int> SaveChangesInBatchAsync<TEntity>(IEnumerable<TEntity> entities, ILogger? logger = null, int batchSize = 200, bool efAcceptAllChangesOnSuccess = false)
      where TEntity : class
    {
      var changeTrackerSettings = new ChangeTrackerSettings(this);

      var optimizedChangeTrackerSettings = new ChangeTrackerSettings();
      optimizedChangeTrackerSettings.ApplySettings(this);
      var totalActualRecordsSaved = await BatchDataSaver.SaveChangesInBatchAsync<TEntity>(async (entityName, totalEstimatedRecords) =>
       {
         var affectedRecordCount = 0;
         for (var currentRecorIndex = 0; currentRecorIndex < totalEstimatedRecords; currentRecorIndex += batchSize)
         {
           var batchStartTime = DateTime.UtcNow;
           var batch = entities.Skip(currentRecorIndex).Take(batchSize);
           affectedRecordCount += await BatchDataSaver.ProcessEntityBatchAsync(this, batch, efAcceptAllChangesOnSuccess);
           logger?.LogInformation("Saved batch of {entityName} records in {elapsedTimeInSecs}, approximate remaining items {pendingRecordCount}",
             entityName,
             DateTime.UtcNow.Subtract(batchStartTime).Seconds,
             Math.Abs(totalEstimatedRecords - currentRecorIndex));
         }
         return affectedRecordCount;
       }, entities, logger, batchSize);

      changeTrackerSettings.ApplySettings(this);
      return totalActualRecordsSaved;
    }
  }
}
