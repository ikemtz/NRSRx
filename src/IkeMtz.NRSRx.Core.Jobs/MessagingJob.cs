using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class MessagingJob<TProgram> : JobBase<TProgram, IMessageFunction>, IJob
    where TProgram : class, IJob
  {
    public virtual bool RunContinously { get; set; } = true;

    /// <summary>
    /// Flag to enable Parallel function processing
    /// </summary>
    public virtual bool EnableParallelFunctionProcessing { get; set; } = false;

    public virtual int? SecsBetweenRuns { get; set; }
    public virtual TimeSpan SleepTimeSpan => new(0, 0, SecsBetweenRuns.GetValueOrDefault());
    public override async Task RunFunctions(ILoggerFactory loggerFactory)
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
    }
  }
}
