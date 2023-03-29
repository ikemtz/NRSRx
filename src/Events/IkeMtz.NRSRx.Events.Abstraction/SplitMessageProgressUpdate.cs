namespace IkeMtz.NRSRx.Events.Abstraction
{
  public class SplitMessageProgressUpdate
  {
    public long Passed { get; set; }
    public long Failed { get; set; }
    public long Total { get; set; }
  }
}
