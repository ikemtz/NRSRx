using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class MessagingJob<TProgram> : JobBase<TProgram, IMessageFunction>, IJob
    where TProgram : class, IJob
  {
    public virtual bool RunContinously { get; set; } = true;
    public virtual int? SecsBetweenRuns { get; set; } = null;
    public override async Task RunFunctions(ILoggerFactory? loggerFactory)
    {
      SecsBetweenRuns ??= Configuration.GetValue("SecsBetweenRuns", 60);
      var functions = GetFunctions(loggerFactory);
      var firstRun = true;
      while (RunContinously || firstRun)
      {
        foreach (var func in functions)
        {
          await ScopeFunctionasync(loggerFactory, func);
        }
        firstRun = false;
        Thread.Sleep(new TimeSpan(0, 0, SecsBetweenRuns.GetValueOrDefault()));
      }
    }
  }
}
