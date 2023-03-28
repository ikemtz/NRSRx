using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using IkeMtz.NRSRx.Jobs.Redis;
using IkeMtz.Samples.Models.V1;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Redis.Jobs
{
  public class SchoolCreatedSplitFunction : SplitMessageFunction<SchoolCreatedSplitFunction, School, UpdatedEvent>
  {
    public SchoolCreatedSplitFunction(ILogger<SchoolCreatedSplitFunction> logger, RedisStreamSubscriber<SplitMessage<School>, UpdatedEvent> subscriber)
      : base(logger, subscriber)
    {
      this.EnablePendingMsgProcessing = true;
      this.MessageBufferCount = 100;
    }

    public override Task HandleMessageAsync(SplitMessage<School> entity)
    {
      Logger.LogInformation("Split Sample handled.");
      return Task.CompletedTask;
    }
  }
}
