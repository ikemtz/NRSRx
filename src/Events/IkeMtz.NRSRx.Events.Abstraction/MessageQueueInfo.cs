using System.Collections.Generic;

namespace IkeMtz.NRSRx.Events
{
  public class MessageQueueInfo
  {
    public int? MessageCount { get; set; }
    public int? SubscriberCount { get; set; }
    public int? PendingMsgCount { get; set; }
    public int? DeadLetterMsgCount { get; set; }
    public int AckMessageCount { get; set; }
  }
}
