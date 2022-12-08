using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class MessagingJob<TProgram> : JobBase<TProgram, IMessageFunction>, IJob
    where TProgram : class, IJob
  {
    public virtual bool RunContinously { get; set; } = true;
    public virtual int? SecsBetweenRuns { get; set; }
    public virtual TimeSpan SleepTimeSpan => new(0, 0, SecsBetweenRuns.GetValueOrDefault());
    public override async Task RunFunctions(ILoggerFactory loggerFactory)
    {
      SecsBetweenRuns ??= Configuration.GetValue("SecsBetweenRuns", 60);
      var logger = loggerFactory.CreateLogger<MessagingJob<TProgram>>();
      var functions = GetFunctions(loggerFactory);
      var firstRun = true;
      while (RunContinously || firstRun)
      {
        foreach (var func in functions)
        {
          await ScopeFunctionasync(loggerFactory, func);
        }
        firstRun = false;
        logger.LogInformation("Finished running jobs, going to sleep for {SecsBetweenRuns} seconds.", SecsBetweenRuns);

        if (RunContinously) Thread.Sleep(SleepTimeSpan);
      }
    }
  }
}
