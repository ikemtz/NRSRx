using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  /// <summary>
  /// Represents an abstract messaging job within the NRSRx framework.
  /// </summary>
  /// <typeparam name="TProgram">The type of the program implementing the job.</typeparam>
  public abstract class MessagingJob<TProgram> : JobBase<TProgram, IMessageFunction>, IJob
      where TProgram : class, IJob
  {
    /// <summary>
    /// Gets or sets a value indicating whether the job should run continuously.
    /// </summary>
    public virtual bool RunContinously { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether parallel function processing is enabled.
    /// </summary>
    public virtual bool EnableParallelFunctionProcessing { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of seconds between runs.
    /// </summary>
    public virtual int? SecsBetweenRuns { get; set; }

    /// <summary>
    /// Gets the sleep time span based on the number of seconds between runs.
    /// </summary>
    public virtual TimeSpan SleepTimeSpan => new(0, 0, SecsBetweenRuns.GetValueOrDefault());

    /// <summary>
    /// Runs the functions associated with the job.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to create loggers.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the functions were successful.</returns>
    public override async Task<bool> RunFunctions(ILoggerFactory loggerFactory)
    {
      SecsBetweenRuns ??= Configuration.GetValue("SecsBetweenRuns", 60);
      var logger = loggerFactory.CreateLogger<MessagingJob<TProgram>>();
      var functions = GetFunctions(loggerFactory);
      var firstRun = true;
      var successResult = true;
      while (RunContinously || firstRun)
      {
        if (EnableParallelFunctionProcessing)
        {
          foreach (var func in functions.AsParallel())
          {
            successResult &= await ScopeFunctionAsync(loggerFactory, func);
          }
        }
        else
        {
          foreach (var func in functions)
          {
            successResult &= await ScopeFunctionAsync(loggerFactory, func);
          }
        }

        if (successResult && !string.IsNullOrWhiteSpace(HealthFileLocation))
        {
          File.WriteAllText(HealthFileLocation, DateTime.UtcNow.ToString());
        }

        firstRun = false;
        logger.LogInformation("Finished running jobs, going to sleep for {SecsBetweenRuns} seconds.", SecsBetweenRuns);

        if (RunContinously) Thread.Sleep(SleepTimeSpan);
      }
      return successResult;
    }
  }
}
