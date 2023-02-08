using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.NRSRx.Jobs.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Redis.Jobs
{
  public class SchoolCreatedFunction : MessageFunction<SchoolCreatedFunction, School, CreatedEvent>
  {
    public SchoolCreatedFunction(ILogger<SchoolCreatedFunction> logger, RedisStreamSubscriber<School, CreatedEvent> subscriber)
      : base(logger, subscriber)
    {
      this.EnablePendingMsgProcessing = true;
      this.AutoDeleteIdleConsumers = true;
    }

    public override async Task HandleMessageAsync(School entity)
    {
     _= await Subscriber.GetStreamInfoAsync();
      Logger.LogInformation("Sample handled.");
    }
  }
}
