using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using NRSRx_OData_EF.Models;

namespace NRSRx_OData_EF.Configuration
{
  public class ModelConfiguration : IModelConfiguration
  {
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
    {
      _ = builder.EntitySet<Item>($"{nameof(Item)}s");
    }
  }
}
