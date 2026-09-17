using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.Samples.OData.Controllers.V1
{
  [Route($"odata/v1/Courses")]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  [Authorize]
  [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 6000)]
  public class CoursesController(DatabaseContext databaseContext) : ODataController
  {
    [ProducesResponseType(typeof(ODataEnvelope<Course, Guid>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet()]
    public IQueryable<Course> Get()
    {
      return databaseContext.Courses
        .AsNoTracking();
    }
  }
}
