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
  public class CoursesController : ControllerBase
  {
    private readonly DatabaseContext _databaseContext;
    public CoursesController(DatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    // Get api/Courses
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    public async Task<ActionResult> Get([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Courses
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      return Ok(obj);
    }

    // Post api/Courses
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ValidateModel]
    public async Task<ActionResult> Post([FromBody] Course value)
    {
      var dbContextObject = _databaseContext.Courses.Add(value);
      _ = await _databaseContext.SaveChangesAsync()
          .ConfigureAwait(false);
      return Ok(dbContextObject.Entity);
    }

    // Put api/Courses
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ValidateModel]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] Course value)
    {
      var obj = await _databaseContext.Courses.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      SimpleMapper<Course>.Instance.ApplyChanges(value, obj);
      _ = await _databaseContext.SaveChangesAsync()
          .ConfigureAwait(false);
      return Ok(obj);
    }

    // Put api/Courses
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    public async Task<ActionResult> Delete([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Courses.FirstOrDefaultAsync(t => t.Id == id)
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
