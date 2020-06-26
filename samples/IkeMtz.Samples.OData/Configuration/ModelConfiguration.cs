using IkeMtz.Samples.OData.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.Samples.OData.Configuration
{
  public class ModelConfiguration : IModelConfiguration
  {
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
    {
      _ = builder.EntitySet<Item>($"{nameof(Item)}s");
      _ = builder.EntitySet<SubItemA>($"{nameof(SubItemA)}s");
      _ = builder.EntityType<Item>().Collection.Function("NoLimit")
        .ReturnsCollectionFromEntitySet<Item>($"{nameof(Item)}s");
    }
  }
}
