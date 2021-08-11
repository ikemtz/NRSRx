using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.OData.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.OData.Controllers.V1
{
  [ApiVersion(VersionDefinitions.v1_0)]
  [Authorize]
  [ODataRoutePrefix("Items")]
  [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 6000)]
  public class ItemsController : ODataController
  {
    private readonly IDatabaseContext _databaseContext;

    public ItemsController(IDatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    [ODataRoute]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<Item, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = All)]
    public IEnumerable<Item> Get()
    {
      return _databaseContext.Items
        .AsNoTracking();
    }

    [ODataRoute("NoLimit")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<Item, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 500, AllowedQueryOptions = All)]
    [HttpGet]
    public IQueryable<Item> NoLimit()
    {
      return _databaseContext.Items
        .AsNoTracking();
    }

    public ActionResult Delete([FromODataUri] Guid key)
    {
      return key != default ? Ok() : NotFound();
    }
  }
}
