namespace IkeMtz.NRSRx.Core.Jobs
{
  public interface IFunction
  {
    /// <summary>
    /// The higher priorty functions get run first (ordered by descending). 
    /// </summary>
    int? SequencePriority { get; }
    // Should return true if successful
    Task<bool> RunAsync();
  }
}
