using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Jobs
{
  internal class SecondFunction : IFunction
  {
    public ILogger<SecondFunction> Logger { get; }
    public SecondFunction(ILogger<SecondFunction> logger)
    {
      Logger = logger;
    }
    public Task<bool> RunAsync()
    {
      Console.WriteLine("Running Second");
      Logger.LogInformation("Running Second");
      return Task.FromResult(true);
    }
  }
}
