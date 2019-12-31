using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Tests.OData.Controllers.V1
{
  public class PostalState: IIdentifiable<string>
  {
    public string Id { get; set; }
    public string Name { get; set; }
  }
}
