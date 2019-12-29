using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace IkeMtz.NRSRx.Core.Tests.OData.Controllers.V1
{
  [ApiVersion("1.0")]
  [ODataRoutePrefix("PostalStates")]
  [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 30)]
  public class PostalStatesController : ODataController
  {
    public PostalStatesController()
    {
    }

    [ODataRoute]
    [ProducesResponseType(typeof(ODataValue<IEnumerable<PostalState>>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = All)]
    public IEnumerable<PostalState> Get()
    {
      return PostalStatesColleciton;
    }
    private static readonly PostalState[] PostalStatesColleciton = new[] {
        new PostalState(){Id="AL", Name="Alabama"},
        new PostalState(){Id="AK", Name="Alaska"},
        new PostalState(){Id="AZ", Name="Arizona"},
        new PostalState(){Id="AR", Name="Arkansas"},
        new PostalState(){Id="CA", Name="California"},
        new PostalState(){Id="CO", Name="Colorado"},
        new PostalState(){Id="CT", Name="Connecticut"},
        new PostalState(){Id="DE", Name="Delaware"},
        new PostalState(){Id="DC", Name="District Of Columbia"},
        new PostalState(){Id="FL", Name="Florida"},
        new PostalState(){Id="GA", Name="Georgia"},
        new PostalState(){Id="HI", Name="Hawaii"},
        new PostalState(){Id="ID", Name="Idaho"},
        new PostalState(){Id="IL", Name="Illinois"},
        new PostalState(){Id="IN", Name="Indiana"},
        new PostalState(){Id="IA", Name="Iowa"},
        new PostalState(){Id="KS", Name="Kansas"},
        new PostalState(){Id="KY", Name="Kentucky"},
        new PostalState(){Id="LA", Name="Louisiana"},
        new PostalState(){Id="ME", Name="Maine"},
        new PostalState(){Id="MD", Name="Maryland"},
        new PostalState(){Id="MA", Name="Massachusetts"},
        new PostalState(){Id="MI", Name="Michigan"},
        new PostalState(){Id="MN", Name="Minnesota"},
        new PostalState(){Id="MS", Name="Mississippi"},
        new PostalState(){Id="MO", Name="Missouri"},
        new PostalState(){Id="MT", Name="Montana"},
        new PostalState(){Id="NE", Name="Nebraska"},
        new PostalState(){Id="NV", Name="Nevada"},
        new PostalState(){Id="NH", Name="New Hampshire"},
        new PostalState(){Id="NJ", Name="New Jersey"},
        new PostalState(){Id="NM", Name="New Mexico"},
        new PostalState(){Id="NY", Name="New York"},
        new PostalState(){Id="NC", Name="North Carolina"},
        new PostalState(){Id="ND", Name="North Dakota"},
        new PostalState(){Id="OH", Name="Ohio"},
        new PostalState(){Id="OK", Name="Oklahoma"},
        new PostalState(){Id="OR", Name="Oregon"},
        new PostalState(){Id="PA", Name="Pennsylvania"},
        new PostalState(){Id="RI", Name="Rhode Island"},
        new PostalState(){Id="SC", Name="South Carolina"},
        new PostalState(){Id="SD", Name="South Dakota"},
        new PostalState(){Id="TN", Name="Tennessee"},
        new PostalState(){Id="TX", Name="Texas"},
        new PostalState(){Id="UT", Name="Utah"},
        new PostalState(){Id="VT", Name="Vermont"},
        new PostalState(){Id="VA", Name="Virginia"},
        new PostalState(){Id="WA", Name="Washington"},
        new PostalState(){Id="WV", Name="West Virginia"},
        new PostalState(){Id="WI", Name="Wisconsin"},
        new PostalState(){Id="WY", Name="Wyoming"},
    };
  }
}
