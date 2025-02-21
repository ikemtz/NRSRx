using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Tests
{
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(TestApiVersionDefinitions.V1_0)]
  [ApiController]
  public class TestController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get(ApiVersion apiVersion)
    {
      var result = new PingResult(apiVersion)
      {
        Name = $"NRSRx Test Controller",
        Build = this.GetBuildNumber()
      };
      return Ok(result);
    }
    [HttpPost]
    [ValidateModel]
    public IActionResult IsValid(TestModel model)
    {
      return Ok(model);
    }
  }
}
