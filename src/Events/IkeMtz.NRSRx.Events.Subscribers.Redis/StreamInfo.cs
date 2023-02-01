namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  public class Consumer
  {
    public string Name { get; set; }
    public int PendingMsgCount { get; set; }
    public long IdleTimeInMs { get; set; }
  }
}
