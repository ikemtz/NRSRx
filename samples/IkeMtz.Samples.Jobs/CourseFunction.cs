using IkeMtz.NRSRx.Jobs.Cron;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IkeMtz.Samples.Jobs
{
  internal class CourseFunction(
    ILogger<CourseFunction> logger,
    TimeProvider timeProvider,
    ICronJobStateProvider cronJobStateProvider,
    DatabaseContext databaseContext) : CronFunction<CourseFunction>(logger, timeProvider, cronJobStateProvider)
  {
    public DatabaseContext DatabaseContext { get; } = databaseContext;

    public override int? SequencePriority { get; } = 100;

    public override string CronExpression { get; set; } = "*/5 * * * *";

    public override async Task<bool> ExecuteAsync()
    {
      _ = DatabaseContext.Courses.Add(
        new Course
        {
          Num = Guid.NewGuid().ToString()[..4],
          Title = Guid.NewGuid().ToString()[..6],
          Description = Guid.NewGuid().ToString()[..20],
          Department = Guid.NewGuid().ToString()[0..25],
          AvgScore = new Random().NextDouble(),
          PassRate = new Random().Next(),
        });
      var result = await DatabaseContext.SaveChangesAsync();
      Logger.LogInformation("Getting courses record count from database");
      var count = await DatabaseContext.Courses.CountAsync();
      Logger.LogInformation("Courses found: {count}", count);
      return result == 1;
    }
  }
}
