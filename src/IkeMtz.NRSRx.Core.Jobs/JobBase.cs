using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  /// <summary>
  /// Base class for defining a job in the NRSRx framework.
  /// </summary>
  /// <typeparam name="TProgram">The type of the program.</typeparam>
  /// <typeparam name="TFunctionType">The type of the function.</typeparam>
  public abstract class JobBase<TProgram, TFunctionType>
      where TProgram : class, IJob
      where TFunctionType : IFunction
  {
    /// <summary>
    /// Gets or sets the job host.
    /// </summary>
    public virtual IHost JobHost { get; set; }

    /// <summary>
    /// Gets the location of the health file.
    /// </summary>
    public virtual string? HealthFileLocation { get; }

    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public virtual IConfiguration Configuration { get; set; }

    /// <summary>
    /// Gets the configuration for the job.
    /// </summary>
    /// <returns>The configuration.</returns>
    public virtual IConfiguration GetConfig()
    {
      return ConfigurationFactory<TProgram>.Create();
    }

    /// <summary>
    /// Runs the job asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public Task<bool> RunAsync()
    {
      _ = this.SetupHost();
      var loggerFactory = JobHost.Services.GetRequiredService<ILoggerFactory>();
      return RunFunctions(loggerFactory);
    }

    /// <summary>
    /// Runs the functions asynchronously.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public virtual async Task<bool> RunFunctions(ILoggerFactory loggerFactory)
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
      return successResult;
    }

    /// <summary>
    /// Gets the functions to run.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>An ordered enumerable of function metadata.</returns>
    public virtual IOrderedEnumerable<FunctionMetadata> GetFunctions(ILoggerFactory? loggerFactory)
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
          return new FunctionMetadata
          {
            Type = type,
            Name = type.Name,
            SequencePriority = t.SequencePriority ?? 0
          };
        }).OrderByDescending(t => t.SequencePriority);
    }

    /// <summary>
    /// Scopes and runs a function asynchronously.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="func">The function metadata.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public virtual async Task<bool> ScopeFunctionAsync(ILoggerFactory loggerFactory, FunctionMetadata func)
    {
      var logger = loggerFactory.CreateLogger(func.GetType());
      var startTime = DateTime.UtcNow;
      using (logger.BeginScope("Function {functionName}", func.Name))
      {
        logger.LogInformation("Function {functionName} start time: {startTime} UTC", func.Name, startTime);
        bool result;
        try
        {
          result = await RunFunctionAsync(func, logger);
        }
        catch (Exception x)
        {
          logger.LogError(x, "An unhandled exception has occurred while executing function: {functionName}", func.Name);
          return false;
        }
        var endTime = DateTime.UtcNow;
        var durationInSecs = endTime.Subtract(startTime).TotalSeconds;
        logger.LogInformation("Function {functionName} completed time: {endTime} UTC", func.Name, endTime);
        logger.LogInformation("Function {functionName} completed in {durationInSecs} secs", func.Name, durationInSecs);
        return result;
      }
    }

    /// <summary>
    /// Sets up the host.
    /// </summary>
    /// <param name="setupMiscDependencies">An action to set up miscellaneous dependencies.</param>
    /// <returns>The configured host.</returns>
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

    /// <summary>
    /// Sets up logging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    [ExcludeFromCodeCoverage]
    public virtual void SetupLogging(IServiceCollection services) { }

    /// <summary>
    /// Sets up dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    [ExcludeFromCodeCoverage]
    public virtual IServiceCollection SetupDependencies(IServiceCollection services) { return services; }

    /// <summary>
    /// Sets up the functions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public abstract IServiceCollection SetupFunctions(IServiceCollection services);

    /// <summary>
    /// Sets up the user provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public virtual IServiceCollection SetupUserProvider(IServiceCollection services)
    {
      return services.AddSingleton<ICurrentUserProvider, SystemUserProvider>();
    }

    /// <summary>
    /// Runs a function asynchronously.
    /// </summary>
    /// <param name="func">The function metadata.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating success.</returns>
    public virtual async Task<bool> RunFunctionAsync(FunctionMetadata func, ILogger logger)
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
      return result;
    }
  }
}
