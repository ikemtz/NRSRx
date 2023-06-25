using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class JobBase<TProgram, TFunctionType>
    where TProgram : class, IJob
    where TFunctionType : IFunction
  {
    public virtual IHost JobHost { get; set; }
    public virtual string? HealthFileLocation { get; }
    public virtual IConfiguration Configuration { get; set; }

    public virtual IConfiguration GetConfig()
    {
      return ConfigurationFactory<TProgram>.Create();
    }

    public async Task RunAsync()
    {
      _ = this.SetupHost();
      var loggerFactory = JobHost.Services.GetRequiredService<ILoggerFactory>();
      await RunFunctions(loggerFactory);
    }

    public virtual async Task RunFunctions(ILoggerFactory loggerFactory)
    {
      var functions = GetFunctions(loggerFactory);
      var successResult = true;
      foreach (var func in functions)
      {
        successResult &= await ScopeFunctionAsync(loggerFactory, func);
      }
      if (successResult && !string.IsNullOrWhiteSpace(HealthFileLocation))
      {
        File.WriteAllText(HealthFileLocation, DateTime.UtcNow.ToString());
      }
    }

    public virtual IOrderedEnumerable<FunctionMetaData> GetFunctions(ILoggerFactory? loggerFactory)
    {
      var jobLogger = loggerFactory?.CreateLogger(GetType());
      using var functionScope = JobHost.Services.CreateScope();
      var functions = functionScope.ServiceProvider.GetServices<TFunctionType>();
      var functionTypeName = typeof(TFunctionType).Name;
      var functionCount = functions.Count();
      jobLogger?.LogInformation("Found {functionCount} executable {functionTypeName} functions", functionCount, functionTypeName);
      for (var i = 0; i < functionCount; i++)
      {
        var functionName = functions.ElementAt(i);
        jobLogger?.LogInformation("[{i}] {functionName}", i, functionName);
      }
      return functions
        .Select(t =>
        {
          var type = t.GetType();
          return new FunctionMetaData
          {
            Type = type,
            Name = type.Name,
            SequencePriority = t.SequencePriority ?? 0
          };
        }).OrderByDescending(t => t.SequencePriority);
    }

    public virtual async Task<Boolean> ScopeFunctionAsync(ILoggerFactory loggerFactory, FunctionMetaData func)
    {
      var logger = loggerFactory.CreateLogger(func.GetType());
      var startTime = DateTime.UtcNow;
      using (logger.BeginScope("Function {functionName}", func.Name))
      {
        logger.LogInformation("Function {functionName} start time: {startTime} UTC", func.Name, startTime);
        try
        {
          await RunFunctionAsync(func, logger);
        }
        catch (Exception x)
        {
          logger.LogError(x, "An unhandled exception has occured while executing function: {functionName}", func.Name);
          return false;
        }
        var endTime = DateTime.UtcNow;
        var durationInSecs = endTime.Subtract(startTime).TotalSeconds;
        logger.LogInformation("Function {functionName} completed time: {endTime} UTC", func.Name, endTime);
        logger.LogInformation("Function {functionName} completed in {durationInSecs} secs", func.Name, durationInSecs);
        return true;
      }
    }

    public virtual IHost SetupHost(Action<IServiceCollection>? setupMiscDependencies = null)
    {
      if (Configuration == null || JobHost == null)
      {
        Configuration = this.GetConfig();
        JobHost = Host.CreateDefaultBuilder()
           .ConfigureServices((services) =>
           {
             SetupLogging(services);
             _ = SetupUserProvider(services);
             _ = SetupDependencies(services);
             _ = SetupFunctions(services);
             setupMiscDependencies?.Invoke(services);
           })
           .Build();
      }
      return JobHost;
    }
    [ExcludeFromCodeCoverage]
    public virtual void SetupLogging(IServiceCollection services) { }
    [ExcludeFromCodeCoverage]
    public virtual IServiceCollection SetupDependencies(IServiceCollection services) { return services; }
    public abstract IServiceCollection SetupFunctions(IServiceCollection services);

    public virtual IServiceCollection SetupUserProvider(IServiceCollection services)
    {
      return services.AddSingleton<ICurrentUserProvider, SystemUserProvider>();
    }

    public virtual async Task RunFunctionAsync(FunctionMetaData func, ILogger logger)
    {
      logger.LogInformation("Starting {functionName} function.", func.Name);
      using var functionScope = JobHost.Services.CreateScope();
      var function = functionScope.ServiceProvider.GetService(func.Type) as IFunction;
      var result = await function.RunAsync();
      if (!result)
      {
        logger.LogError("An error occurred in {functionName}.", func.Name);
      }
      logger.LogInformation("Ending {functionName} function.", func.Name);
    }
  }
}
