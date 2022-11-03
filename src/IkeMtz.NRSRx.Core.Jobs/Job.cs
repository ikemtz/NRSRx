using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class Job : IJob
  {
    public abstract string Name { get; }
    public IHost JobHost { get; private set; }
    public IConfiguration Configuration { get; protected set; }
    public virtual IHost SetupHost()
    {
      if (Configuration == null || JobHost == null)
      {
        Configuration = this.GetConfig();
        JobHost = Host.CreateDefaultBuilder()
           .ConfigureServices((services) =>
           {
             SetupLogging(services);
             _ = SetupDependencies(services);
             _ = SetupJobs(services);
           })
           .Build();
      }
      return JobHost;
    }

    public virtual async Task RunAsync()
    {
      _ = this.SetupHost();
      var loggerFactory = JobHost.Services.GetService<ILoggerFactory>();
      var jobLogger = loggerFactory?.CreateLogger(this.GetType());
      //  await JobHost.RunAsync();
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
          logger.LogInformation("Function {functionName} completed time: {endTime} UTC", functionName, endTime);
          logger.LogInformation("Function {functionName} completed in {durationInSecs} secs", functionName, durationInSecs);
        }
      }
    }

    public virtual async Task RunFunctionAsync(string functionName, IFunction x, ILogger? logger)
    {
      logger?.LogInformation("Starting {functionName} function.", functionName);
      var result = await x.RunAsync();
      if (!result)
      {
        logger?.LogError("An error occurred in {functionName}.", functionName);
      }

      logger?.LogInformation("Ending {functionName} function.", functionName);

    }

    public virtual IConfiguration GetConfig()
    {
      var configBuilder = new ConfigurationBuilder();
      if (File.Exists("appsettings.json"))
      {
        _ = configBuilder.AddJsonFile("appsettings.json");
      }
      var config = configBuilder
         .AddUserSecrets(this.GetType().Assembly)
         .AddEnvironmentVariables()
         .Build();
      return config;

    }
    public virtual IServiceCollection SetupDependencies(IServiceCollection services) { return services; }
    public virtual void SetupLogging(IServiceCollection services) { }
    public virtual IServiceCollection SetupJobs(IServiceCollection services) { return services; }
  }
}
