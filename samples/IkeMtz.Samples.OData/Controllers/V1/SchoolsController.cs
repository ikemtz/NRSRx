using System;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
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
  public class SchoolsController(DatabaseContext databaseContext) : ODataController
  {
    [ProducesResponseType(typeof(ODataEnvelope<School, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet]
    public IQueryable<School> Get()
    {
      return databaseContext.Schools
        .AsNoTracking();
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<School, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 500, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("odata/v1/schools/nolimit")]
    public IQueryable<School> NoLimit()
    {
      return databaseContext.Schools
        .AsNoTracking();
    }

    [HttpDelete()]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), Status404NotFound)]
    public async Task<ActionResult> Delete([FromODataUri] Guid key)
    {
      if (Guid.Empty == key)
      {
        return BadRequest(new ProblemDetails { Title = "Invalid id was provided" });
      }
      var school = await databaseContext.Schools.FirstOrDefaultAsync(t => t.Id == key);
      if (school == null)
      {
        return NotFound(new ProblemDetails { Title = "No School found with the provided id" });
      }
      databaseContext.Schools.Remove(school);
      var result = await databaseContext.SaveChangesAsync();
      return result == 1 ? NoContent() : StatusCode(Status500InternalServerError,
        new ProblemDetails { Title = "An error occurred while deleting the school" });
    }
  }
}
