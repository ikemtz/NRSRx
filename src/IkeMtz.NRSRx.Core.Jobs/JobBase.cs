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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public virtual IHost JobHost { get; set; }
    public virtual IConfiguration Configuration { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public virtual IConfiguration GetConfig()
    {
      var configBuilder = new ConfigurationBuilder();
      if (File.Exists("appsettings.json"))
      {
        _ = configBuilder.AddJsonFile("appsettings.json");
      }
      var config = configBuilder
         .AddUserSecrets(typeof(TProgram).Assembly)
         .AddEnvironmentVariables()
         .Build();
      return config;
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
      foreach (var func in functions)
      {
        await ScopeFunctionasync(loggerFactory, func);
      }
    }
    public virtual IOrderedEnumerable<TFunctionType> GetFunctions(ILoggerFactory? loggerFactory)
    {
      var jobLogger = loggerFactory?.CreateLogger(GetType());
      var functions = JobHost.Services.GetServices<TFunctionType>();
      var functionTypeName = typeof(TFunctionType).Name;
      var functionCount = functions.Count();
      jobLogger?.LogInformation("Found {functionCount} executable {functionTypeName} functions", functionCount, functionTypeName);
      for (var i = 0; i < functionCount; i++)
      {
        var functionName = functions.ElementAt(i);
        jobLogger?.LogInformation("[{i}] {functionName}", i, functionName);
      }
      return functions.OrderByDescending(t => t.SequencePriority);
    }

    public virtual async Task ScopeFunctionasync(ILoggerFactory loggerFactory, TFunctionType func)
    {
      var functionName = func.GetType().Name;
      var logger = loggerFactory.CreateLogger(func.GetType());
      var startTime = DateTime.UtcNow;
      using (logger.BeginScope("Function {functionName}", functionName))
      {
        logger.LogInformation("Function {functionName} start time: {startTime} UTC", functionName, startTime);
        try
        {
          await RunFunctionAsync(functionName, func, logger);
        }
        catch (Exception x)
        {
          logger.LogError(x, "An unhandled exception has occured while executing function: {functionName}", functionName);
        }
        var endTime = DateTime.UtcNow;
        var durationInSecs = endTime.Subtract(startTime).TotalSeconds;
        logger.LogInformation("Function {functionName} completed time: {endTime} UTC", functionName, endTime);
        logger.LogInformation("Function {functionName} completed in {durationInSecs} secs", functionName, durationInSecs);
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

    public virtual async Task RunFunctionAsync(string functionName, TFunctionType x, ILogger logger)
    {
      logger.LogInformation("Starting {functionName} function.", functionName);
      var result = await x.RunAsync();
      if (!result)
      {
        logger.LogError("An error occurred in {functionName}.", functionName);
      }
      logger.LogInformation("Ending {functionName} function.", functionName);
    }
  }
}
