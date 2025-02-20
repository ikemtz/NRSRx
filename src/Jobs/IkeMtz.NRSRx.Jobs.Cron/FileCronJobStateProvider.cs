using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Jobs.Cron
{
  /// <summary>
  /// Provides functionality to manage the state of a cron job using the file system.
  /// </summary>
  public class FileCronJobStateProvider : ICronJobStateProvider
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCronJobStateProvider"/> class.
    /// </summary>
    /// <param name="cronJobStateDirectory">The directory where cron job state files are stored.</param>
    /// <param name="timeProvider">The time provider to use for getting the current time.</param>
    public FileCronJobStateProvider(DirectoryInfo cronJobStateDirectory, TimeProvider timeProvider)
    {
      StateDirectory = cronJobStateDirectory;
      TimeProvider = timeProvider;
      if (!cronJobStateDirectory.Exists)
      {
        cronJobStateDirectory.Create();
      }
    }

    /// <summary>
    /// Gets or sets the directory where cron job state files are stored.
    /// </summary>
    public DirectoryInfo StateDirectory { get; set; }

    /// <summary>
    /// Gets or sets the time provider to use for getting the current UTC time.
    /// </summary>
    public TimeProvider TimeProvider { get; set; }

    /// <summary>
    /// Gets the state of the specified cron job.
    /// </summary>
    /// <param name="cronFunction">The cron function whose state is to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the state of the cron job.</returns>
    public async Task<CronJobState> GetCronJobStateAsync(CronFunction cronFunction)
    {
      var filePath = GetStateFilePath(cronFunction);
      if (File.Exists(filePath))
      {
        var json = await File.ReadAllTextAsync(filePath);
        return JsonConvert.DeserializeObject<CronJobState>(json) ?? new CronJobState();
      }
      else return new CronJobState();
    }

    /// <summary>
    /// Updates the state of the specified cron job.
    /// </summary>
    /// <param name="cronFunction">The cron function whose state is to be updated.</param>
    /// <param name="nextExecutionDateTimeUtc">The next execution date and time in UTC.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated state of the cron job.</returns>
    public async Task<CronJobState> UpdateCronJobStateAsync(CronFunction cronFunction, DateTimeOffset nextExecutionDateTimeUtc)
    {
      var filePath = GetStateFilePath(cronFunction);
      var cronJobState = new CronJobState
      {
        LastRunDateTimeUtc = TimeProvider.GetUtcNow(),
        NextRunDateTimeUtc = nextExecutionDateTimeUtc
      };
      await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(cronJobState));
      return cronJobState;
    }

    private string GetStateFilePath(CronFunction cronFunction)
    {
      return $"{StateDirectory.FullName}/{cronFunction.GetType().Name}.state.json";
    }
  }
}
