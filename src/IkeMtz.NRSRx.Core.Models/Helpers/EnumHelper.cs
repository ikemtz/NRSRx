using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Helpers
{
  public static class EnumHelper
  {
    public static IEnumerable<(TKeyType Id, string Name)> ToIEnumerable<TEnum, TKeyType>() where TEnum : Enum
    {
      var tEnumType = typeof(TEnum);
      return Enum.GetValues(tEnumType)
         .Cast<TKeyType>()
         .Select(t => (Id: t, Name: Enum.GetName(tEnumType, t)));
    }
    public static IEnumerable<(int Id, string Name)> ToIEnumerable<TEnum>() where TEnum : Enum
    {
      return ToIEnumerable<TEnum, int>();
    }
  }
}
