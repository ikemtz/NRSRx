using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Tests
{
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersion(TestApiVersionDefinitions.v1_0)]
  [ApiController]
  public class TestController : ControllerBase
  {
    [HttpPost]
    [ValidateModel]
    public IActionResult IsValid(TestModel model)
    {
      return Ok();
    }
  }

  public class Startup : CoreWebApiStartup
  {
    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    public override string MicroServiceTitle => "";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
