using System.Collections.Generic;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.Samples.OData.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;


namespace IkeMtz.Samples.OData.Configuration
{
  public class ODataModelProvider : BaseODataModelProvider
  {
    public static IEdmModel GetV1EdmModel()
    {
      var builder = new ODataConventionModelBuilder();
      _ = builder.EntitySet<Item>($"{nameof(Item)}s");
      _ = builder.EntitySet<SubItemA>($"{nameof(SubItemA)}s");
      _ = builder.EntityType<Item>()
        .Collection
        .Function("NoLimit")
        .ReturnsCollectionFromEntitySet<Item>($"{nameof(Item)}s");
      return builder.GetEdmModel();
    }

    public override IDictionary<string, IEdmModel> GetModels()
    {
      return new Dictionary<string, IEdmModel>
      {
        { "v1", GetV1EdmModel() }
      };
    }
  }
}
