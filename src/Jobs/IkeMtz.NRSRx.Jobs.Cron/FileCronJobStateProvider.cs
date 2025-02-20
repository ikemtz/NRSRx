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
    /// <typeparam name="TCronFunction">The type of the cron function.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the state of the cron job.</returns>
    public async Task<CronJobState> GetCronJobStateAsync<TCronFunction>() where TCronFunction : class
    {
      var filePath = GetStateFilePath<TCronFunction>();
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
    /// <typeparam name="TCronFunction">The type of the cron function.</typeparam>
    /// <param name="nextExecutionDateTimeUtc">The next execution date and time in UTC.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated state of the cron job.</returns>
    public async Task<CronJobState> UpdateCronJobStateAsync<TCronFunction>(DateTimeOffset nextExecutionDateTimeUtc) where TCronFunction : class
    {
      var filePath = GetStateFilePath<TCronFunction>();
      var cronJobState = new CronJobState
      {
        LastRunDateTimeUtc = TimeProvider.GetUtcNow(),
        NextRunDateTimeUtc = nextExecutionDateTimeUtc
      };
      await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(cronJobState));
      return cronJobState;
    }

    /// <summary>
    /// Gets the file path for the state of the specified cron job.
    /// </summary>
    /// <typeparam name="TCronFunction">The type of the cron function.</typeparam>
    /// <returns>The file path for the state of the specified cron job.</returns>
    private string GetStateFilePath<TCronFunction>() where TCronFunction : class
    {
      return $"{StateDirectory.FullName}/{typeof(TCronFunction).Name}.state.json";
    }
  }
}
