using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.WebApi.Tests.SampleWeb;
using IkeMtz.Samples.Models;
using IkeMtz.Samples.WebApi.Data;
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
  public class MultiTenantItemsController : ControllerBase
  {
    private readonly IDatabaseContext _databaseContext;
    public MultiTenantItemsController(IDatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    // Get api/MultiTenantItems
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(Item))]
    [SampleTenantFilter()]
    public async Task<ActionResult> Get([FromQuery] Guid id, [FromQuery] string tid)
    {
      var obj = await _databaseContext.Items
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tid)
        .ConfigureAwait(false);
      return Ok(obj);
    }
  }
}
