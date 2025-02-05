using IkeMtz.NRSRx.Core;

namespace IkeMtz.NRSRx.Jobs.Core
{
  /// <summary>
  /// Provides the current job name as the current user id, useful for working with <see cref="T:IkeMtz.NRSRx.Core.EntityFramework.AuditableDbContext"/>.
  /// </summary>
  /// <typeparam name="Job">The type of job.</typeparam>
  public class JobUserProvider<Job> : ICurrentUserProvider
      where Job : IJob
  {
    /// <inheritdoc/>
    public string? GetCurrentUserId(string? defaultValue = null)
    {
      return typeof(Job).Name;
    }
  }
}
