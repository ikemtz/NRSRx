using System.Collections.Generic;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  public class MessageQueueInfo
  {
    public int? MessageCount { get; set; }
    public int? SubscriberCount { get; set; }
    public int? DeadLetterCount { get; set; }
  }
}
