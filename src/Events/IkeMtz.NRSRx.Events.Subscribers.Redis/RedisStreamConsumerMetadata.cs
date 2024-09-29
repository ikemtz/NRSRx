namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  /// <summary>
  /// Represents metadata for a Redis stream consumer.
  /// </summary>
  public class RedisStreamConsumerMetadata
  {
    /// <summary>
    /// Gets or sets the name of the consumer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the count of pending messages for the consumer.
    /// </summary>
    public int PendingMsgCount { get; set; }

    /// <summary>
    /// Gets or sets the idle time in milliseconds for the consumer.
    /// </summary>
    public long IdleTimeInMs { get; set; }
  }
}
