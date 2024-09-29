using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs
{
  /// <summary>
  /// Represents an abstract job within the NRSRx framework.
  /// </summary>
  /// <typeparam name="TProgram">The type of the program implementing the job.</typeparam>
  public abstract class Job<TProgram> : JobBase<TProgram, IFunction>, IJob
      where TProgram : class, IJob
  {
    /// <summary>
    /// Sets up the dependencies required by the job.
    /// </summary>
    /// <param name="services">The service collection to add the dependencies to.</param>
    /// <returns>The service collection with the dependencies added.</returns>
    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return services;
    }
  }
}
