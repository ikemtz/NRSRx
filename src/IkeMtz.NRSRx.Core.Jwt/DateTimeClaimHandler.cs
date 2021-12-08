using System;

namespace IkeMtz.NRSRx.Core.Jwt
{
  public class UtcDateTimeClaimParser
  {
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static DateTime ParseClaimValue(int value)
    {
      return Epoch.AddSeconds(value);

    }
  }
}
