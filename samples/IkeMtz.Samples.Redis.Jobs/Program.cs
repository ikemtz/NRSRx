using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IkeMtz.Samples.Redis.Jobs
{
  public class Program : MessagingJob<Program>
  {
    public ConnectionMultiplexer RedisConnectionMultiplexer { get; private set; }
    public static async Task Main()
    {
      var prog = new Program
      {
        RunContinously = false
      };
      await prog.RunAsync();
    }

    public override IServiceCollection SetupFunctions(IServiceCollection services)
    {
      return services.AddSingleton<IMessageFunction, SchoolCreatedFunction>();
    }

    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      var redisConnectionString = Configuration.GetValue<string>("REDIS_CONNECTION_STRING");
      RedisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
      return services.AddSingleton<RedisStreamSubscriber<School, CreatedEvent>>((x) =>
        new SchoolCreatedSubscriber(RedisConnectionMultiplexer));
    }

    [ExcludeFromCodeCoverage()]
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
