using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.OData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.OData.Controllers.V1
{
  [ApiVersion("1.0")]
  [Authorize]
  [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 6000)]
  public class ItemsController : ODataController
  {
    private readonly IDatabaseContext _databaseContext;

    public ItemsController(IDatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    [ProducesResponseType(typeof(ODataEnvelope<Item, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet]
    public ActionResult<IQueryable<Item>> Get()
    {
      return Ok(_databaseContext.Items
        .AsNoTracking());
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<Item, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 500, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("nolimit")]
    public ActionResult<IQueryable<Item>> NoLimit()
    {
      return Ok(_databaseContext.Items
        .AsNoTracking());
    }

    [HttpDelete]
    public ActionResult Delete([FromODataUri] Guid key)
    {
      return key != Guid.Empty ? Ok() : NotFound();
    }
  }
}
