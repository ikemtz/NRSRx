using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.Redis;
using IkeMtz.Samples.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.Events.Redis.Controllers.V1
{
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  public class ItemsController : ControllerBase
  {

    // Post api/Items
    [HttpPost]
    [ProducesResponseType(Status200OK, Type = typeof(Item))]
    [ValidateModel]
    public async Task<ActionResult> Post([FromBody] Item value, [FromServices] RedisStreamPublisher<Item, CreatedEvent> publisher)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
      var result = await publisher.PublishAsync(value)
#pragma warning restore CA1062 // Validate arguments of public methods
        .ConfigureAwait(false);
      return Ok(result);
    }

    // Put api/Items
    [HttpPut]
    [ProducesResponseType(Status200OK, Type = typeof(Item))]
    [ValidateModel]
    public async Task<ActionResult> Put([FromQuery] Guid id, [FromBody] Item value, [FromServices] RedisStreamPublisher<Item, UpdatedEvent> publisher)
    {
#pragma warning disable CA1062 // Validate arguments of public methods
      value.Id = id;
      var result = await publisher.PublishAsync(value)
#pragma warning restore CA1062 // Validate arguments of public methods
        .ConfigureAwait(false);
      return Ok(result);
    }

    // Put api/Items
    [HttpDelete]
    [ProducesResponseType(Status200OK, Type = typeof(Item))]
    public async Task<ActionResult> Delete([FromQuery] Guid id, [FromServices] RedisStreamPublisher<Item, DeletedEvent> publisher)
    {
      var value = new Item { Id = id };
#pragma warning disable CA1062 // Validate arguments of public methods
      var result = await publisher.PublishAsync(value)
#pragma warning restore CA1062 // Validate arguments of public methods
        .ConfigureAwait(false);
      return Ok(result);
    }
  }
}
