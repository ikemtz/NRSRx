using System;

namespace IkeMtz.NRSRx.Core.Jwt
{
  /// <summary>
  /// Provides methods to convert between DateTime and Unix epoch time.
  /// </summary>
  public static class EpochDateConverter
  {
    /// <summary>
    /// Represents the Unix epoch start time (January 1, 1970, 00:00:00 UTC).
    /// </summary>
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Converts a double value representing seconds since the Unix epoch to a DateTime.
    /// </summary>
    /// <param name="value">The number of seconds since the Unix epoch.</param>
    /// <returns>A DateTime representing the specified number of seconds since the Unix epoch.</returns>
    public static DateTime FromDouble(double value)
    {
      return Epoch.AddSeconds(value);
    }

    /// <summary>
    /// Converts a DateTime to a double value representing seconds since the Unix epoch.
    /// </summary>
    /// <param name="value">The DateTime to convert.</param>
    /// <returns>The number of seconds since the Unix epoch.</returns>
    public static double ToDouble(DateTime value)
    {
      return value.Subtract(Epoch).TotalSeconds;
    }
  }
}
