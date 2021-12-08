using System;

namespace IkeMtz.NRSRx.Core.Jwt
{
  public class EpochDateConverter
  {
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static DateTime FromDouble(double value)
    {
      return Epoch.AddSeconds(value);
    }
    public static double ToDouble(DateTime value)
    {
      return value.Subtract(Epoch).TotalSeconds;
    }
  }
}
