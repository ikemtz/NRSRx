using IkeMtz.NRSRx.Core.Jobs;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddFunction<TFunction>(this IServiceCollection services)
    where TFunction : class, IFunction
    {
      services = services
        .AddScoped<TFunction>()
        .AddScoped<IFunction, TFunction>();
      return services;
    }

    public static IServiceCollection AddMessageFunction<TFunction>(this IServiceCollection services)
    where TFunction : class, IMessageFunction
    {
      services = services
        .AddScoped<TFunction>()
        .AddScoped<IMessageFunction, TFunction>();
      return services;
    }
  }
}
