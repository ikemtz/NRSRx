using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis.Publishers;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace IkeMtz.Samples.Events.Redis
{
  public class Startup : CoreWebApiStartup
  {
    public override string MicroServiceTitle => $"{nameof(IkeMtz.Samples.Events.Redis)} WebApi Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;

    public Startup(IConfiguration configuration) : base(configuration) { }

    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) => this.SetupConsoleLogging(app);

    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder healthChecks)
    {
      _ = healthChecks.AddRedis(Configuration.GetValue<string>("REDIS_CONNECTION_STRING"));
    }

    [ExcludeFromCodeCoverage]
    public override void SetupPublishers(IServiceCollection services)
    {
      var redisConnectionString = Configuration.GetValue<string>("REDIS_CONNECTION_STRING");
      var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
      _ = services.AddSingleton<ISimplePublisher<Course, CreatedEvent, RedisValue>>((x) => new CourseCreatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<Course, UpdatedEvent, RedisValue>>((x) => new CourseUpdatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<Course, DeletedEvent, RedisValue>>((x) => new CourseDeletedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<Student, CreatedEvent, RedisValue>>((x) => new StudentCreatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<Student, UpdatedEvent, RedisValue>>((x) => new StudentUpdatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<Student, DeletedEvent, RedisValue>>((x) => new StudentDeletedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<School, CreatedEvent, RedisValue>>((x) => new SchoolCreatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<School, UpdatedEvent, RedisValue>>((x) => new SchoolUpdatedPublisher(connectionMultiplexer));
      _ = services.AddSingleton<ISimplePublisher<School, DeletedEvent, RedisValue>>((x) => new SchoolDeletedPublisher(connectionMultiplexer));
    }
  }
}
