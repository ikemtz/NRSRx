using System.Collections.Generic;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.Samples.OData.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OData.Edm;

namespace IkeMtz.Samples.OData.Configuration
{
  public class ODataModelProvider : BaseODataModelProvider
  {
    public static IEdmModel GetV1EdmModel() =>
      ODataEntityModelFactory(builder =>
      {
        _ = builder.EntitySet<Item>($"{nameof(Item)}s");
        _ = builder.EntitySet<SubItemA>($"{nameof(SubItemA)}s");
        _ = builder.EntityType<Item>()
          .Collection
          .Function("nolimit")
          .ReturnsCollectionFromEntitySet<Item>($"{nameof(Item)}s");
      });

    public override IDictionary<ApiVersionDescription, IEdmModel> GetModels() =>
        new Dictionary<ApiVersionDescription, IEdmModel>
        {
          [ApiVersionFactory(1, 0)] = GetV1EdmModel(),
        };
  }
}
