using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Helpers
{
  public static class EnumHelper
  {
    public static IEnumerable<(TKeyType Id, string Name)> ToIEnumerable<TEnum, TKeyType>()
      where TEnum : Enum
    {
      var tEnumType = typeof(TEnum);
      return Enum.GetValues(tEnumType)
         .Cast<TKeyType>()
         .Select(t => (Id: t, Name: Enum.GetName(tEnumType, t)));
    }
    public static IEnumerable<(int Id, string Name)> ToIEnumerable<TEnum>()
      where TEnum : Enum
    {
      return ToIEnumerable<TEnum, int>();
    }

    public static IEnumerable<TEnumValueType> ConvertEnumValues<TEnum, TEnumValueType>()
      where TEnum : struct, Enum
      where TEnumValueType : IEnumValue<int>, new()
    {
      var tEnum = typeof(TEnum);
      return Enum.GetValues(tEnum).Cast<int>().Select(t => new TEnumValueType { Id = t, Name = Enum.GetName(tEnum, t) });
    }

    public static IEnumerable<TEnumValueType> ConvertEnumValues<TEnum, TIdentityType, TEnumValueType>()
      where TEnum : struct, Enum
      where TIdentityType : IComparable
      where TEnumValueType : IEnumValue<TIdentityType>, new()
    {
      var tEnum = typeof(TEnum);
      return Enum.GetValues(tEnum).Cast<int>().Select(t => new TEnumValueType { Id = (TIdentityType)(t as dynamic), Name = Enum.GetName(tEnum, t) });
    }
  }
}
