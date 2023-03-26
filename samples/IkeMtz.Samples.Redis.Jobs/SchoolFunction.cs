using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.NRSRx.Jobs.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Redis.Jobs
{
  public class SchoolCreatedFunction : MessageFunction<SchoolCreatedFunction, School, CreateEvent>
  {
    public SchoolCreatedFunction(ILogger<SchoolCreatedFunction> logger, RedisStreamSubscriber<School, CreateEvent> subscriber)
      : base(logger, subscriber)
    {
      this.EnablePendingMsgProcessing = true;
      this.MessageBufferCount = 100;
    }

    public override Task HandleMessageAsync(School entity)
    {
      Logger.LogInformation("Sample handled.");
      return Task.CompletedTask;
    }
  }
}
