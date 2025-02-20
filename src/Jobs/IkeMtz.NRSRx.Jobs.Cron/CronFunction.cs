using IkeMtz.NRSRx.Jobs.Core;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace IkeMtz.NRSRx.Jobs.Cron
{
  /// <summary>
  /// Represents a scheduled function that can be executed based on a condition.
  /// </summary>
  public abstract class CronFunction : IFunction
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CronFunction"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param> 
    /// <param name="timeProvider">The time provider instance.</param>
    /// <param name="cronJobStateProvider">The cron job state provider instance.</param>
    public CronFunction(ILogger<CronFunction> logger, TimeProvider timeProvider, ICronJobStateProvider cronJobStateProvider)
    {
      Logger = logger;
      TimeProvider = timeProvider;
      CronJobStateProvider = cronJobStateProvider;
      CronJobState = cronJobStateProvider.GetCronJobStateAsync(this).Result;
    }

    /// <summary>
    /// Gets or sets the state of the cron job.
    /// </summary>
    public CronJobState CronJobState { get; set; }

    /// <summary>
    /// Gets or sets the schedule for the cron function.
    /// </summary>
    public CrontabSchedule? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the cron expression for the function.
    /// For guidance visit: https://crontab.guru/
    /// </summary>
    public abstract string CronExpression { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the function should execute on startup.
    /// </summary>
    public bool ExecuteOnStartup { get; set; }

    /// <inheritdoc/>
    public virtual int? SequencePriority { get; } = 100;

    /// <summary>
    /// Gets the logger for the cron function.
    /// </summary>
    public ILogger<CronFunction> Logger { get; }

    /// <summary>
    /// Gets the time provider for the cron function.
    /// </summary>
    public TimeProvider TimeProvider { get; }

    /// <summary>
    /// Gets or sets the cron job state provider.
    /// </summary>
    public ICronJobStateProvider CronJobStateProvider { get; set; }

    /// <summary>
    /// Framework function to control CRON job execution.
    /// WARNING: ** It's not recommended that you override this function. **
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the function executed successfully.</returns>
    public virtual async Task<bool> RunAsync()
    {
      Schedule ??= CrontabSchedule.Parse(CronExpression);
      CronJobState.NextRunDateTimeUtc ??= Schedule.GetNextOccurrence(CronJobState.LastRunDateTimeUtc.GetValueOrDefault(TimeProvider.GetUtcNow()).UtcDateTime);
      if (ExecuteOnStartup || CronJobState.NextRunDateTimeUtc <= TimeProvider.GetUtcNow())
      {
        var nextOccurence = Schedule.GetNextOccurrence(CronJobState.LastRunDateTimeUtc.Value.DateTime);
        CronJobState = await CronJobStateProvider.UpdateCronJobStateAsync(this, nextOccurence);
        ExecuteOnStartup = false;
        Logger.LogInformation("Executing {FunctionName} function, at {LastRunTime}.  Next run time is: {NextRuntime}", GetType().Name, CronJobState.LastRunDateTimeUtc, CronJobState.NextRunDateTimeUtc);
        return await ExecuteAsync();
      }
      else
      {
        Logger.LogInformation("Skipping execution of {FunctionName} function, at {LastRunTime}.  Next run time is: {NextRuntime}", GetType().Name, CronJobState.LastRunDateTimeUtc, CronJobState.NextRunDateTimeUtc);
        return true;
      }
    }

    /// <summary>
    /// Executes the function logic asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the function executed successfully.</returns>
    public abstract Task<bool> ExecuteAsync();
  }
}
