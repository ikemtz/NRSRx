using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{

  /// <summary>
  /// Interface for saving entities in batches to the database.
  /// </summary>
  /// <typeparam name="TDbContext">The type of the database context.</typeparam>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public interface IBatchDataSaver<TDbContext, TEntity>
      where TDbContext : DbContext
      where TEntity : class
  {
    /// <summary>
    /// Saves the changes in batches asynchronously.
    /// </summary>
    /// <param name="dbContextFactory">The factory function to create the database context.</param>
    /// <param name="entities">The collection of entities to be saved.</param>
    /// <param name="logger">The logger instance for logging.</param>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesInBatchAsync(Func<TDbContext> dbContextFactory, IEnumerable<TEntity> entities, ILogger? logger = null, int batchSize = 200);
  }
}
