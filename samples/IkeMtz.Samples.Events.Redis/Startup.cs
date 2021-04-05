using System.Reflection;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models;
using IkeMtz.Samples.Events.Redis.Publishers;
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

    public override void SetupPublishers(IServiceCollection services)
    {
      var redisConnectionString = Configuration.GetValue<string>("REDIS_CONNECTION_STRING");
      var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
      services.AddSingleton<ISimplePublisher<Item, CreatedEvent, RedisValue>>((x) => new ItemCreatedPublisher(connectionMultiplexer));
      services.AddSingleton<ISimplePublisher<Item, UpdatedEvent, RedisValue>>((x) => new ItemUpdatedPublisher(connectionMultiplexer));
      services.AddSingleton<ISimplePublisher<Item, DeletedEvent, RedisValue>>((x) => new ItemDeletedPublisher(connectionMultiplexer));
    }
  }
}
