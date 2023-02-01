using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  public class RedisSubscriberOptions
  {
    /// <summary>
    /// This should be set to either StreamPosition.Beginning or StreamConstants.NewMessages
    /// </summary>
    public RedisValue StartPosition { get; set; } = StreamPosition.NewMessages;

    /// <summary>
    /// The amount of time in (ms) to wait before considering a consumer Idle.
    /// Default value 10 Minutes
    /// </summary>
    public int IdleTimeSpanInMilliseconds { get; set; } = 600_000;

    /// <summary>
    /// Maximum amount of times to retry to process a message.
    /// </summary>
    public int MaxMessageProcessRetry { get; set; } = 3;

    /// <summary>
    /// The consumer group name for the subscriber
    /// </summary>
    public string? ConsumerGroupName { get; set; }
  }
}
