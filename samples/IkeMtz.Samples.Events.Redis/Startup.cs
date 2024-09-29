using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis
{
  public class Startup(IConfiguration configuration) : CoreWebApiStartup(configuration)
  {
    public override string ServiceTitle => $"{nameof(IkeMtz.Samples.Events.Redis)} WebApi Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;

    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) => this.SetupConsoleLogging(app);

    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder healthChecks)
    {
      _ = healthChecks.AddRedis(Configuration.GetValue<string>("REDIS_CONNECTION_STRING"));
    }

    [ExcludeFromCodeCoverage]
    public override void SetupPublishers(IServiceCollection services)
    {
      var redisConnectionString = Configuration.GetValue<string>("REDIS_CONNECTION_STRING");
      var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
      _ = services.AddSingleton<IPublisher<Course, CreatedEvent>>((x) => new RedisStreamPublisher<Course, CreatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<Course, UpdatedEvent>>((x) => new RedisStreamPublisher<Course, UpdatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<Course, DeletedEvent>>((x) => new RedisStreamPublisher<Course, DeletedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<Student, CreatedEvent>>((x) => new RedisStreamPublisher<Student, CreatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<Student, UpdatedEvent>>((x) => new RedisStreamPublisher<Student, UpdatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<Student, DeletedEvent>>((x) => new RedisStreamPublisher<Student, DeletedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<School, CreatedEvent>>((x) => new RedisStreamPublisher<School, CreatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<School, UpdatedEvent>>((x) => new RedisStreamPublisher<School, UpdatedEvent>(connectionMultiplexer));
      _ = services.AddSingleton<IPublisher<School, DeletedEvent>>((x) => new RedisStreamPublisher<School, DeletedEvent>(connectionMultiplexer));
    }
  }
}
