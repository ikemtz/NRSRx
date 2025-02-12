using System;

namespace IkeMtz.NRSRx.Core.Unigration.Fakes
{
  /// <summary>
  /// Provides a fake implementation of the TimeProvider for testing purposes.
  /// </summary>
  public class FakeTimeProvider : TimeProvider
  {
    /// <summary>
    /// Gets the DateTimeOffset value.
    /// </summary>
    public DateTimeOffset DateTimeOffset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeTimeProvider"/> class with the specified DateTimeOffset.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset value to be used by the fake time provider.</param>
    public FakeTimeProvider(DateTimeOffset dateTimeOffset)
    {
      this.DateTimeOffset = dateTimeOffset;
    }

    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    /// <returns>The UTC date and time specified in the constructor.</returns>
    public override DateTimeOffset GetUtcNow()
    {
      return DateTimeOffset;
    }
  }
}
