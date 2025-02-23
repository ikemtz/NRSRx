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
  public class StudentsController : ControllerBase
  {
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<StudentsController> logger;

    public StudentsController(DatabaseContext databaseContext, ILogger<StudentsController> logger)
    {
      _databaseContext = databaseContext;
      this.logger = logger;
    }

    // Get api/Students
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    public async Task<ActionResult> Get([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Students
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      return Ok(obj);
    }

    // Post api/Students
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ValidateModel]
    public async Task<ActionResult> Post([FromBody] StudentUpsertRequest request)
    {
      var value = SimpleMapper<StudentUpsertRequest, Student>.Instance.Convert(request);
      value.Id = request.Id;
      var dbContextObject = _databaseContext.Students.Add(value);
      _ = await _databaseContext.SaveChangesAsync(logger)
          .ConfigureAwait(false);
      return Ok(dbContextObject.Entity);
    }

    // Put api/Students
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ValidateModel]
    [ValidateMatchingId]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] StudentUpsertRequest request)
    {
      var obj = await _databaseContext.Students.FirstOrDefaultAsync(t => t.Id == id)
        .ConfigureAwait(false);
      SimpleMapper<StudentUpsertRequest, Student>.Instance.ApplyChanges(request, obj);
      _ = await _databaseContext.SaveChangesAsync(logger)
          .ConfigureAwait(false);
      return Ok(obj);
    }

    // Put api/Students
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    public async Task<ActionResult> Delete([FromQuery] Guid id)
    {
      var obj = await _databaseContext.Students.FirstOrDefaultAsync(t => t.Id == id)
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
