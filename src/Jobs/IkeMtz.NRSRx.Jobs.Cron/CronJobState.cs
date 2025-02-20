namespace IkeMtz.NRSRx.Jobs.Cron
{
  /// <summary>
  /// Represents the state of a CronJob, including its last run time, success status, and next run time.
  /// </summary>
  public class CronJobState
  {
    /// <summary>
    /// Gets or sets the last run date and time of the CronJob in UTC.
    /// </summary>
    public DateTimeOffset? LastRunDateTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the next run date and time of the CronJob in UTC.
    /// </summary>
    public DateTimeOffset? NextRunDateTimeUtc { get; set; }
  }
}
