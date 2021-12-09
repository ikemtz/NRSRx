using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
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
    public async Task<ActionResult> Post([FromBody] Student value, [FromServices] RedisStreamPublisher<Student, CreatedEvent> publisher)
    {
      var result = await publisher.PublishAsync(value)
        .ConfigureAwait(false);
      return Ok(result);
    }

    // Put api/Students
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] Student value, [FromServices] RedisStreamPublisher<Student, UpdatedEvent> publisher)
    {
      value.Id = id;
      var result = await publisher.PublishAsync(value)
        .ConfigureAwait(false);
      return Ok(result);
    }

    // Delete api/Students
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Student))]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Delete([FromQuery] Guid id, [FromServices] RedisStreamPublisher<Student, DeletedEvent> publisher)
    {
      var value = new Student { Id = id };
      var result = await publisher.PublishAsync(value)
        .ConfigureAwait(false);
      return Ok(result);
    }
  }
}
