using System.Collections.Generic;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OData.Edm;
using V1 = IkeMtz.Samples.Models.V1;

namespace IkeMtz.Samples.OData.Configuration
{
  public class ODataModelProvider : BaseODataModelProvider
  {
    public static IEdmModel GetV1EdmModel() =>
      ODataEntityModelFactory(builder =>
      {
        _ = builder.EntitySet<V1.Course>($"{nameof(V1.Course)}s");
        _ = builder.EntitySet<V1.School>($"{nameof(V1.School)}s");
        _ = builder.EntitySet<V1.Student>($"{nameof(V1.Student)}s");
        _ = builder.EntityType<V1.Student>()
          .Collection
          .Function("nolimit")
          .ReturnsCollectionFromEntitySet<V1.Student>($"{nameof(V1.Student)}s");
      });

    public override IDictionary<ApiVersionDescription, IEdmModel> GetModels() =>
        new Dictionary<ApiVersionDescription, IEdmModel>
        {
          [ApiVersionFactory(1, 0)] = GetV1EdmModel(),
        };
  }
}
