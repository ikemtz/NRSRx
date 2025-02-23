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
  public class CoursesController(DatabaseContext databaseContext, ILogger<CoursesController> logger) : ControllerBase
  {
    private readonly DatabaseContext _databaseContext = databaseContext;
    private readonly ILogger<CoursesController> logger = logger;

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
    public async Task<ActionResult> Post([FromBody] CourseUpsertRequest request)
    {
      var value = SimpleMapper<CourseUpsertRequest, Course>.Instance.Convert(request);
      value.Id = request.Id;
      var dbContextObject = _databaseContext.Courses.Add(value);
      _ = await _databaseContext.SaveChangesAsync(logger)
          .ConfigureAwait(false);
      return Ok(dbContextObject.Entity);
    }

    // Put api/Courses
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ValidateModel]
    [ValidateMatchingId]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] CourseUpsertRequest request)
    {
      var obj = await _databaseContext.Courses.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      SimpleMapper<CourseUpsertRequest, Course>.Instance.ApplyChanges(request, obj);
      _ = await _databaseContext.SaveChangesAsync(logger)
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
        _ = await _databaseContext.SaveChangesAsync(logger)
            .ConfigureAwait(false);
        return Ok(obj);
      }
      return NotFound("Invalid Id");
    }
  }
}
