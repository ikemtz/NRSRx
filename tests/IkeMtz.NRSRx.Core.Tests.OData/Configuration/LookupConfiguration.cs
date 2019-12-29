using IkeMtz.NRSRx.Core.Tests.OData.Controllers.V1;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Tests.OData.Configuration
{
  public class LookupsConfiguration : IModelConfiguration
  {
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
    { 
      builder.EntitySet<PostalState>("PostalStates"); 
    }
  }
}
