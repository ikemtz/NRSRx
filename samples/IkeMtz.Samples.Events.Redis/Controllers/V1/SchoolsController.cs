using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.Events.Redis.Controllers.V1
{
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  public class SchoolsController : ControllerBase
  {
    // Post api/Schools
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Post([FromBody] SchoolUpsertRequest request, [FromServices] IPublisher<School, CreatedEvent> publisher)
    {
      var value = SimpleMapper<SchoolUpsertRequest, School>.Instance.Convert(request);
      value.Id = request.Id;
      await publisher.PublishAsync(value)
        .ConfigureAwait(false);
      return Ok();
    }

    // Put api/Schools
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ValidateModel]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] SchoolUpsertRequest request, [FromServices] IPublisher<School, UpdatedEvent> publisher)
    {
      var value = SimpleMapper<SchoolUpsertRequest, School>.Instance.Convert(request);
      value.Id = id;
      await publisher.PublishAsync(value)
         .ConfigureAwait(false);
      return Ok();
    }

    // Delete api/Schools
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(School))]
    [ExcludeFromCodeCoverage()] //Need to figure out why method is not getting code coverage
    public async Task<ActionResult> Delete([FromQuery] Guid id, [FromServices] IPublisher<School, DeletedEvent> publisher)
    {
      var value = new School { Id = id };
      await publisher.PublishAsync(value)
         .ConfigureAwait(false);
      return Ok();
    }
  }
}
