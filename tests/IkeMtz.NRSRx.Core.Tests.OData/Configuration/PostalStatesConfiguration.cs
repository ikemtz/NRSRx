using IkeMtz.NRSRx.Core.OData;
using IkeMtz.NRSRx.Core.Tests.OData.Controllers.V1;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Tests.OData.Configuration
{
  public class PostalStatesConfiguration : IModelConfiguration
  {
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
    {
      ODataConfigurationBuilder<PostalState, string>.EntitySetBuilder(builder);
    }
  }
}
