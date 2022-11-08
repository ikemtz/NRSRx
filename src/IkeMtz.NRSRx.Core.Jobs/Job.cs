using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class Job<TProgram> : JobBase<TProgram>, IJob
    where TProgram : class, IJob
  {
    public override async Task RunFunctions(ILoggerFactory? loggerFactory)
    {
      var jobLogger = loggerFactory?.CreateLogger(GetType());
      var functions = JobHost.Services.GetServices<IFunction>();
      var functionCount = functions.Count();
      jobLogger?.LogInformation("Found {functionCount} executable functions", functionCount);
      foreach (var func in functions)
      {
        var functionName = func.GetType().Name;
        var logger = loggerFactory?.CreateLogger(func.GetType());
        var startTime = DateTime.UtcNow;
        using (logger?.BeginScope("Function {functionName}", functionName))
        {
          logger?.LogInformation("Function {functionName} start time: {startTime} UTC", functionName, startTime);
          try
          {
            await RunFunctionAsync(functionName, func, logger);
          }
          catch (Exception x)
          {
            logger?.LogError(x, "An unhandled exception has occured while executing function: {functionName}", functionName);
          }
          var endTime = DateTime.UtcNow;
          var durationInSecs = endTime.Subtract(startTime).TotalSeconds;
          logger?.LogInformation("Function {functionName} completed time: {endTime} UTC", functionName, endTime);
          logger?.LogInformation("Function {functionName} completed in {durationInSecs} secs", functionName, durationInSecs);
        }
      }
    }


    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services) { return services; }
  }
}
