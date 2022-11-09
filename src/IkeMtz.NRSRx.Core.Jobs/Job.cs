using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public abstract class Job<TProgram> : JobBase<TProgram, IFunction>, IJob
    where TProgram : class, IJob
  {
    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return services;
    }
  }
}
