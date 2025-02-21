using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SchoolsController> logger;

    public SchoolsController(DatabaseContext databaseContext, ILogger<SchoolsController> logger)
    {
      _databaseContext = databaseContext;
      this.logger = logger;
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
    public async Task<ActionResult> Post([FromBody] SchoolUpsertRequest request)
    {
      var value = SimpleMapper<SchoolUpsertRequest, School>.Instance.Convert(request);
      var dbContextObject = _databaseContext.Schools.Add(value);
      _ = await _databaseContext.SaveChangesAsync(logger)
          .ConfigureAwait(false);
      return Ok(dbContextObject.Entity);
    }

    // Put api/Schools
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ValidateModel]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] SchoolUpsertRequest request)
    {
      var obj = await _databaseContext.Schools.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      SimpleMapper<SchoolUpsertRequest, School>.Instance.ApplyChanges(request, obj);
      _ = await _databaseContext.SaveChangesAsync(logger)
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
        _ = await _databaseContext.SaveChangesAsync(logger)
            .ConfigureAwait(false);
        return Ok(obj);
      }
      return NotFound("Invalid Id");
    }
  }
}
