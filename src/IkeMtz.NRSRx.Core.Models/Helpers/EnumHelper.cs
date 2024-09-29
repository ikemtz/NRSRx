using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Helpers
{
  /// <summary>
  /// Provides helper methods for working with enums.
  /// </summary>
  public static class EnumHelper
  {
    /// <summary>
    /// Converts an enum to an enumerable of tuples containing the identifier and name.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <typeparam name="TKeyType">The type of the identifier.</typeparam>
    /// <returns>An enumerable of tuples containing the identifier and name.</returns>
    public static IEnumerable<(TKeyType Id, string Name)> ToIEnumerable<TEnum, TKeyType>()
      where TEnum : Enum
    {
      var tEnumType = typeof(TEnum);
      return Enum.GetValues(tEnumType)
         .Cast<TKeyType>()
         .Select(t => (Id: t, Name: Enum.GetName(tEnumType, t)));
    }

    /// <summary>
    /// Converts an enum to an enumerable of tuples containing the identifier and name, with the identifier as an integer.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <returns>An enumerable of tuples containing the identifier and name.</returns>
    public static IEnumerable<(int Id, string Name)> ToIEnumerable<TEnum>()
      where TEnum : Enum
    {
      return ToIEnumerable<TEnum, int>();
    }

    /// <summary>
    /// Converts enum values to a collection of a specified type that implements <see cref="IEnumValue{T}"/>.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <typeparam name="TEnumValueType">The type that implements <see cref="IEnumValue{T}"/>.</typeparam>
    /// <returns>A collection of the specified type containing the enum values.</returns>
    public static IEnumerable<TEnumValueType> ConvertEnumValues<TEnum, TEnumValueType>()
      where TEnum : struct, Enum
      where TEnumValueType : IEnumValue<int>, new()
    {
      var tEnum = typeof(TEnum);
      return Enum.GetValues(tEnum).Cast<int>().Select(t => new TEnumValueType { Id = t, Name = Enum.GetName(tEnum, t) });
    }

    /// <summary>
    /// Converts enum values to a collection of a specified type that implements <see cref="IEnumValue{T}"/> with a specified identifier type.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <typeparam name="TIdentityType">The type of the identifier.</typeparam>
    /// <typeparam name="TEnumValueType">The type that implements <see cref="IEnumValue{T}"/>.</typeparam>
    /// <returns>A collection of the specified type containing the enum values.</returns>
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
