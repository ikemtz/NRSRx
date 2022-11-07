using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public interface IMessageFunction<TEntity> : IMessageFunction
    where TEntity : class, IIdentifiable
  {
    // Should return true if successful
    Task<bool> ProcessMessage(TEntity entity);
  }
  public interface IMessageFunction
  {
  }
}
