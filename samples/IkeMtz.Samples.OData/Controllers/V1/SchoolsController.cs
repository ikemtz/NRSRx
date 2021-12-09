using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Data;
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
  public class SchoolsController : ODataController
  {
    private readonly DatabaseContext _databaseContext;

    public SchoolsController(DatabaseContext databaseContext)
    {
      _databaseContext = databaseContext;
    }

    [ProducesResponseType(typeof(ODataEnvelope<School, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet]
    public IQueryable<School> Get()
    {
      return _databaseContext.Schools
        .AsNoTracking();
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<School, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 500, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("odata/v1/schools/nolimit")]
    public IQueryable<School> NoLimit()
    {
      return _databaseContext.Schools
        .AsNoTracking();
    }

    [HttpDelete]
    public ActionResult Delete([FromODataUri] Guid key)
    {
      return key != Guid.Empty ? Ok() : NotFound();
    }
  }
}
