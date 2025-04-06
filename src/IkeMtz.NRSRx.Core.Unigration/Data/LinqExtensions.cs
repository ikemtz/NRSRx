using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides extension methods for LINQ queries.
  /// </summary>
  public static class LinqExtensions
  {
    /// <summary>
    /// Returns a random element from the source collection asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An IQueryable&lt;T&gt; to return a random element from.</param>
    /// <param name="recordCount">The number of elements in the source collection. If null, the count will be determined automatically.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a random element from the source collection.</returns>
    /// <exception cref="ArgumentException">Thrown when the source collection is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the count is less than 2.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the source collection is empty.</exception>
    public static async Task<T> RandomAsync<T>(this IQueryable<T> source, int? recordCount = null)
    {
      if (source == null)
      {
        throw new ArgumentException("Source collection is null.");
      }
      else if (recordCount != null && recordCount < 2)
      {
        throw new ArgumentOutOfRangeException(nameof(recordCount), "Count must be greater than 1.");
      }
      recordCount ??= await source.CountAsync();
      if (recordCount == 0)
      {
        throw new InvalidOperationException("Source collection is empty.");
      }
      var random = new Random(DateTime.Now.Microsecond);
      var index = random.Next(0, recordCount.Value);
      return await source.Skip(index).FirstAsync();
    }
  }
}
