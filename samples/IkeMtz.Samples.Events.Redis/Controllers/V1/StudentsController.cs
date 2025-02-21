using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.Events.Redis.Controllers.V1
{
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  public class StudentsController : ControllerBase
  {
    // Post api/Students
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Post([FromBody] StudentUpsertRequest request, [FromServices] IPublisher<Student, CreatedEvent> publisher)
    {
      var value = SimpleMapper<StudentUpsertRequest, Student>.Instance.Convert(request);
      value.Id = request.Id;
      await publisher.PublishAsync(value)
         .ConfigureAwait(false);
      return Ok();
    }

    // Put api/Students
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] StudentUpsertRequest request, [FromServices] IPublisher<Student, UpdatedEvent> publisher)
    {
      var value = SimpleMapper<StudentUpsertRequest, Student>.Instance.Convert(request);
      value.Id = id;
      await publisher.PublishAsync(value)
           .ConfigureAwait(false);
      return Ok();
    }

    // Delete api/Students
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Delete([FromQuery] Guid id, [FromServices] IPublisher<Student, DeletedEvent> publisher)
    {
      var value = new Student { Id = id };
      await publisher.PublishAsync(value)
           .ConfigureAwait(false);
      return Ok();
    }
  }
}
