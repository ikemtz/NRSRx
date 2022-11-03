using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Jobs
{
  internal class FirstFunction : IFunction
  {
    public ILogger<FirstFunction> Logger { get; }
    public FirstFunction(ILogger<FirstFunction> logger)
    {
      Logger = logger;
    }


    public Task<bool> RunAsync()
    {
      Console.WriteLine("Running First");
      Logger.LogInformation("Running First");
      return Task.FromResult(true);
    }
  }
}
