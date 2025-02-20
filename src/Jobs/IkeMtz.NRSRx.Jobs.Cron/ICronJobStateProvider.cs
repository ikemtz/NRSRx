namespace IkeMtz.NRSRx.Jobs.Cron
{
  /// <summary>
  /// Common interface to get and update the state of a cron job.
  /// </summary>
  public interface ICronJobStateProvider
  {
    /// <summary>
    /// Gets the state of the specified cron job.
    /// </summary>
    /// <returns>The state of the specified cron job.</returns>
    Task<CronJobState> GetCronJobStateAsync<TCronFunction>() where TCronFunction : class;

    /// <summary>
    /// Updates the state of the cron job with the next execution date and time.
    /// </summary> 
    /// <param name="nextExecutionDateTimeUtc">The next execution date and time in UTC.</param>
    /// <returns>The updated state of the cron job.</returns>
    Task<CronJobState> UpdateCronJobStateAsync<TCronFunction>(DateTimeOffset nextExecutionDateTimeUtc) where TCronFunction : class;
  }
}
