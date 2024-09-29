namespace IkeMtz.NRSRx.Events
{
  /// <summary>
  /// Represents information about the current status of a message queue.
  /// </summary>
  public class MessageQueueInfo
  {
    /// <summary>
    /// Gets or sets the total number of messages in the queue.
    /// </summary>
    public int? MessageCount { get; set; }

    /// <summary>
    /// Gets or sets the number of subscribers to the queue.
    /// </summary>
    public int? SubscriberCount { get; set; }

    /// <summary>
    /// Gets or sets the number of pending messages in the queue.
    /// </summary>
    public int? PendingMsgCount { get; set; }

    /// <summary>
    /// Gets or sets the number of dead-letter messages in the queue.
    /// </summary>
    public int? DeadLetterMsgCount { get; set; }

    /// <summary>
    /// Gets or sets the number of acknowledged messages in the queue.
    /// </summary>
    public int AckMessageCount { get; set; }

    /// <summary>
    /// Gets or sets the stream key associated with the queue.
    /// </summary>
    public string StreamKey { get; set; }
  }
}
