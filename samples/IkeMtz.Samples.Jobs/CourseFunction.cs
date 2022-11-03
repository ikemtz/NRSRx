using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.Samples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Jobs
{
  internal class CourseFunction : IFunction
  {
    public ILogger Logger { get; }
    public DatabaseContext DatabaseContext { get; }
    public CourseFunction(ILogger<CourseFunction> logger, DatabaseContext databaseContext)
    {
      Logger = logger;
      DatabaseContext = databaseContext;
    }
    public async Task<bool> RunAsync()
    {
      Logger.LogInformation("Getting courses record count from database");
      var count = await DatabaseContext.Courses.CountAsync();
      Logger.LogInformation("Courses found: {count}", count);
      return true;
    }
  }
}
