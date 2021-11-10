using System;
using System.Collections.Generic;
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
  [ApiVersion(VersionDefinitions.v1_0)]
  [Authorize]
  [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 6000)]
  public class ItemsController : ODataController
  {
    private readonly IDatabaseContext _databaseContext;

    public ItemsController(IDatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<Item, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    public IEnumerable<Item> Get()
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
