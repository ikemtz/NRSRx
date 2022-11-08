namespace IkeMtz.NRSRx.Core.Jobs
{
  public interface IFunction
  {
    // Should return true if successful
    Task<bool> RunAsync();
  }
}
