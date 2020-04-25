using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using IkeMtz.Samples.OData.Models;

namespace IkeMtz.Samples.OData.Configuration
{
  public class ModelConfiguration : IModelConfiguration
  {
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
    {
      _ = builder.EntitySet<Item>($"{nameof(Item)}s");
    }
  }
}
