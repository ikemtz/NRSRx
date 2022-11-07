using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class JobBase<TProgram>
    where TProgram : class, IJob
  {
    public virtual IHost JobHost { get; set; }
    public virtual IConfiguration Configuration { get; set; }

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
      var loggerFactory = JobHost.Services.GetService<ILoggerFactory>();
      await RunFunctions(loggerFactory);
    }
    public abstract Task RunFunctionAsync(string functionName, IFunction x, ILogger? logger);
    public abstract Task RunFunctions(ILoggerFactory? loggerFactory);

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
             _ = SetupFunctions(services);
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
  }
}
