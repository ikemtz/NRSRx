using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public interface IBatchDataSaver<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class
  {
    Task<int> SaveChangesInBatchAsync(Func<TDbContext> dbContextFactory, IEnumerable<TEntity> entities, ILogger? logger = null, int batchSize = 200);

  }
}
