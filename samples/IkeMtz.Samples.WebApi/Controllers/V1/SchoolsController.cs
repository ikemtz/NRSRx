using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.Samples.Models.V1;
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
  public class SchoolsController : ControllerBase
  {
    private readonly DatabaseContext _databaseContext;
    public SchoolsController(DatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    // Get api/Schools
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    public async Task<ActionResult> Get([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Schools
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      return Ok(obj);
    }

    // Post api/Schools
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ValidateModel]
    public async Task<ActionResult> Post([FromBody] School value)
    {
      var dbContextObject = _databaseContext.Schools.Add(value);
      _ = await _databaseContext.SaveChangesAsync()
          .ConfigureAwait(false);
      return Ok(dbContextObject.Entity);
    }

    // Put api/Schools
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ValidateModel]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] School value)
    {
      var obj = await _databaseContext.Schools.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      SimpleMapper<School>.Instance.ApplyChanges(value, obj);
      _ = await _databaseContext.SaveChangesAsync()
          .ConfigureAwait(false);
      return Ok(obj);
    }

    // Put api/Schools
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    public async Task<ActionResult> Delete([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Schools.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      if (obj != null)
      {
        _ = _databaseContext.Remove(obj);
        _ = await _databaseContext.SaveChangesAsync()
            .ConfigureAwait(false);
        return Ok(obj);
      }
      return NotFound("Invalid Id");
    }
  }
}
