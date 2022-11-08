using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
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
  public class CoursesController : ControllerBase
  {
    // Post api/Courses
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Post([FromBody] CourseUpsertRequest request, [FromServices] RedisStreamPublisher<Course, CreatedEvent> publisher)
    {
      var value = SimpleMapper<CourseUpsertRequest, Course>.Instance.Convert(request);
      value.Id = request.Id;
      await publisher.PublishAsync(value)
       .ConfigureAwait(false);
      return Ok();
    }

    // Put api/Courses
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] CourseUpsertRequest request, [FromServices] RedisStreamPublisher<Course, UpdatedEvent> publisher)
    {
      var value = SimpleMapper<CourseUpsertRequest, Course>.Instance.Convert(request);
      value.Id = id;
      await publisher.PublishAsync(value)
             .ConfigureAwait(false);
      return Ok();
    }

    // Delete api/Courses
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Course))]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Delete([FromQuery] Guid id, [FromServices] RedisStreamPublisher<Course, DeletedEvent> publisher)
    {
      var value = new Course { Id = id };
      await publisher.PublishAsync(value)
            .ConfigureAwait(false);
      return Ok();
    }
  }
}
