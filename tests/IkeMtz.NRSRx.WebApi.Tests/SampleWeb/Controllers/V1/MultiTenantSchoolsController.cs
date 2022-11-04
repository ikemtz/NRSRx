using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.WebApi.Tests.SampleWeb;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.WebApi.Controllers.V1
{
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  [Authorize]
  public class MultiTenantSchoolsController : ControllerBase
  {
    private readonly DatabaseContext _databaseContext;
    public MultiTenantSchoolsController(DatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    // Get api/MultiTenantSchools
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [SampleTenantFilter()]
    public async Task<ActionResult> Get([FromQuery] Guid id, [FromQuery] string tid)
    {
      var obj = await _databaseContext.Schools
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tid)
        .ConfigureAwait(false);
      return Ok(obj);
    }
  }
}
