namespace IkeMtz.NRSRx.Events.Abstraction
{
  /// <summary>
  /// Represents the progress update for a collection of split messages.
  /// This is useful when implementing the Fan Out pattern.
  /// </summary>
  public class SplitMessageProgressUpdate
  {
    /// <summary>
    /// Gets or sets the number of successfully processed messages.
    /// </summary>
    public long Passed { get; set; }

    /// <summary>
    /// Gets or sets the number of failed messages.
    /// </summary>
    public long Failed { get; set; }

    /// <summary>
    /// Gets or sets the total number of messages.
    /// </summary>
    public long Total { get; set; }
  }
}
