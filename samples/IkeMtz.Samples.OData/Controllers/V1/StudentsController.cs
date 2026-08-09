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
  public class StudentsController(DatabaseContext databaseContext) : ODataController
  {
    [ProducesResponseType(typeof(ODataEnvelope<Student, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet]
    public IQueryable<Student> Get()
    {
      return databaseContext.Students
        .AsNoTracking();
    }

    [Produces("application/json")]
    [ProducesResponseType(typeof(ODataEnvelope<School, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 500, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("odata/v1/students/nolimit")]
    public IQueryable<Student> NoLimit()
    {
      return databaseContext.Students
        .AsNoTracking();
    }

    [HttpDelete]
    [ProducesResponseType(Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), Status404NotFound)]
    public async Task<ActionResult> Delete([FromODataUri] Guid key)
    {
      if (Guid.Empty == key)
      {
        return BadRequest(new ProblemDetails { Title = "Invalid id was provided" });
      }
      var student = await databaseContext.Students.FirstOrDefaultAsync(t => t.Id == key);
      if (student == null)
      {
        return NotFound(new ProblemDetails { Title = "No Student found with the provided id" });
      }
      databaseContext.Students.Remove(student);
      var result = await databaseContext.SaveChangesAsync();
      return result == 1 ? NoContent() : StatusCode(Status500InternalServerError,
        new ProblemDetails { Title = "An error occurred while deleting the student" });
    }
  }
}
