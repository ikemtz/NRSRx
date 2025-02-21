using IkeMtz.NRSRx.Jobs.Core;
using IkeMtz.NRSRx.Jobs.Cron;
using Moq;

namespace IkeMtz.NRSRx.Jobs.Unigration
{
  /// <summary>
  /// Provides a mock implementation of the ICronJobStateProvider interface for testing purposes.
  /// </summary>
  public class MockCronJobStateProvider : ICronJobStateProvider
  {
    /// <summary>
    /// Initializes a new instance of the MockCronJobStateProvider class.
    /// </summary>
    /// <param name="timeProvider">The time provider to use for setting initial cron job state.</param>
    public MockCronJobStateProvider(TimeProvider timeProvider)
    {
      Mock = new Mock<ICronJobStateProvider>();
      var cronJobState = new CronJobState
      {
        LastRunDateTimeUtc = timeProvider.GetUtcNow().AddDays(-180),
        NextRunDateTimeUtc = timeProvider.GetUtcNow().AddDays(-1)
      };
      Mock.
        Setup(m => m.GetCronJobStateAsync<It.IsSubtype<IFunction>>())
        .ReturnsAsync(cronJobState);
      Mock
        .Setup(m => m.UpdateCronJobStateAsync<It.IsSubtype<IFunction>>(It.IsAny<DateTimeOffset>()))
        .ReturnsAsync(cronJobState);
    }

    /// <summary>
    /// Gets or sets the mock object for the ICronJobStateProvider interface.
    /// </summary>
    public Mock<ICronJobStateProvider> Mock { get; set; }

    /// <summary>
    /// Gets the cron job state asynchronously.
    /// </summary>
    /// <typeparam name="TCronFunction">The type of the cron function.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cron job state.</returns>
    public Task<CronJobState> GetCronJobStateAsync<TCronFunction>() where TCronFunction : class
    {
      return Mock.Object.GetCronJobStateAsync<TCronFunction>();
    }

    /// <summary>
    /// Updates the cron job state asynchronously.
    /// </summary>
    /// <typeparam name="TCronFunction">The type of the cron function.</typeparam>
    /// <param name="nextExecutionDateTimeUtc">The next execution date and time in UTC.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated cron job state.</returns>
    public Task<CronJobState> UpdateCronJobStateAsync<TCronFunction>(DateTimeOffset nextExecutionDateTimeUtc) where TCronFunction : class
    {
      return Mock.Object.UpdateCronJobStateAsync<TCronFunction>(nextExecutionDateTimeUtc);
    }
  }
}
