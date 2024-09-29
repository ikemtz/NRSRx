using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Provides helper methods and constants for the SimpleMapper.
  /// </summary>
  public static class SimpleMapperHelper
  {
    /// <summary>
    /// Properties to ignore during mapping.
    /// </summary>
    public static readonly string[] IgnoredProperties = { "Id", "CreatedBy", "CreatedOnUtc", "UpdatedBy", "UpdatedOnUtc" };

    /// <summary>
    /// Interfaces to ignore during mapping.
    /// </summary>
    public static readonly string[] IgnoredInterfaces = { nameof(IIdentifiable), typeof(IIdentifiable<>).Name, typeof(ICollection<>).Name };
  }

  /// <summary>
  /// Provides a simple mapper for mapping properties between source and destination entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TSourceEntity">The type of the source entity.</typeparam>
  /// <typeparam name="TDestinationEntity">The type of the destination entity.</typeparam>
  public class SimpleMapper<TSourceEntity, TDestinationEntity> :
    SimpleMapper<TSourceEntity, TDestinationEntity, Guid>
    where TSourceEntity : class
    where TDestinationEntity : class, IIdentifiable<Guid>, new()
  {
  }

  /// <summary>
  /// Provides a simple mapper for mapping properties within the same entity type with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public class SimpleMapper<TEntity> : SimpleMapper<TEntity, TEntity, Guid>
    where TEntity : class, IIdentifiable<Guid>, new()
  {
    internal SimpleMapper()
    {
    }

    /// <summary>
    /// Populates the property mappings for the entity.
    /// </summary>
    /// <returns>A collection of actions to map properties.</returns>
    protected override IEnumerable<Action<TEntity, TEntity>> PopulatePropertyMappings()
    {
      return FilteredSourceProperties
        .Where(prop => prop.CanWrite && prop.CanRead)
        .Select(prop => new Action<TEntity, TEntity>((src, dest) =>
          prop.SetValue(dest, prop.GetValue(src))));
    }
  }

  /// <summary>
  /// Provides a simple mapper for mapping properties between source and destination entities with a specified identifier type.
  /// </summary>
  /// <typeparam name="TSourceEntity">The type of the source entity.</typeparam>
  /// <typeparam name="TDestinationEntity">The type of the destination entity.</typeparam>
  /// <typeparam name="TIdentityType">The type of the identifier.</typeparam>
  public class SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType>
    where TIdentityType : IComparable
    where TSourceEntity : class
    where TDestinationEntity : class, IIdentifiable<TIdentityType>, new()
  {
    private IEnumerable<Action<TSourceEntity, TDestinationEntity>> _propertyMappers;
    private static SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType> _instance;

    /// <summary>
    /// Gets the singleton instance of the SimpleMapper.
    /// </summary>
    public static SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType> Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType>();
          _instance._propertyMappers = _instance.PopulatePropertyMappings();
        }
        return _instance;
      }
    }

    internal SimpleMapper()
    {
    }

    /// <summary>
    /// Gets the filtered source properties for mapping.
    /// </summary>
    protected virtual IEnumerable<PropertyInfo> FilteredSourceProperties => typeof(TSourceEntity).GetRuntimeProperties()
        .Where(prop => !SimpleMapperHelper.IgnoredProperties.Contains(prop.Name, StringComparer.CurrentCulture))
        .Where(prop => !prop.PropertyType.GetTypeInfo().GetInterfaces().Select(t => t.Name).Any(t =>
          SimpleMapperHelper.IgnoredInterfaces.Contains(t, StringComparer.CurrentCulture)));

    /// <summary>
    /// Populates the property mappings between source and destination entities.
    /// </summary>
    /// <returns>A collection of actions to map properties.</returns>
    protected virtual IEnumerable<Action<TSourceEntity, TDestinationEntity>> PopulatePropertyMappings()
    {
      var sourceProperties = FilteredSourceProperties
          .Where(prop => prop.CanRead)
          .ToDictionary(prop => prop.Name);
      return typeof(TDestinationEntity).GetRuntimeProperties()
            .Where(prop => sourceProperties.Keys.Contains(prop.Name, StringComparer.CurrentCulture))
            .Where(prop => prop.CanWrite)
            .Where(prop =>
              prop.PropertyType == sourceProperties.FirstOrDefault(f => f.Key.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase)).Value?.PropertyType)
            .Select(prop => new Action<TSourceEntity, TDestinationEntity>((src, dest) =>
               SetPropertyValue(prop, sourceProperties, src, dest)
            ));
    }

    /// <summary>
    /// Sets the property value from the source entity to the destination entity.
    /// </summary>
    /// <param name="destPropertyInfo">The destination property info.</param>
    /// <param name="sourceProperties">The source properties dictionary.</param>
    /// <param name="src">The source entity.</param>
    /// <param name="dest">The destination entity.</param>
    public static void SetPropertyValue(PropertyInfo destPropertyInfo, Dictionary<string, PropertyInfo> sourceProperties, TSourceEntity src, TDestinationEntity dest)
    {
      var sourceValue = sourceProperties[destPropertyInfo.Name].GetValue(src);
      var destValue = destPropertyInfo.GetValue(dest);
      if ((sourceValue?.Equals(destValue)).GetValueOrDefault()) { return; }
      switch (destPropertyInfo.PropertyType.ToString())
      {
        case "System.Nullable`1[System.Single]":
          destPropertyInfo.SetValue(dest, !float.TryParse(sourceValue?.ToString(), out var floatOutput) ? new float?() : floatOutput);
          break;
        case "System.Single":
          destPropertyInfo.SetValue(dest, !float.TryParse(sourceValue?.ToString(), out floatOutput) ? 0 : floatOutput);
          break;
        case "System.Nullable`1[System.Int32]":
          destPropertyInfo.SetValue(dest, !int.TryParse(sourceValue?.ToString(), out var intOutput) ? new int?() : intOutput);
          break;
        case "System.Int32":
          destPropertyInfo.SetValue(dest, !int.TryParse(sourceValue?.ToString(), out intOutput) ? 0 : intOutput);
          break;
        case "System.Nullable`1[System.Decimal]":
          destPropertyInfo.SetValue(dest, !decimal.TryParse(sourceValue?.ToString(), out var decOutput) ? new decimal?() : decOutput);
          break;
        case "System.Decimal":
          destPropertyInfo.SetValue(dest, !decimal.TryParse(sourceValue?.ToString(), out decOutput) ? 0 : decOutput);
          break;
        case "System.Nullable`1[System.Int64]":
          destPropertyInfo.SetValue(dest, !long.TryParse(sourceValue?.ToString(), out var longOutput) ? new long?() : longOutput);
          break;
        case "System.Int64":
          destPropertyInfo.SetValue(dest, !long.TryParse(sourceValue?.ToString(), out longOutput) ? 0 : longOutput);
          break;
        default:
          destPropertyInfo.SetValue(dest, sourceValue);
          break;
      }
    }

    /// <summary>
    /// Applies changes from the source entity to the destination entity.
    /// </summary>
    /// <param name="sourceEntity">The source entity.</param>
    /// <param name="destinationEntity">The destination entity.</param>
    /// <exception cref="ArgumentNullException">Thrown when sourceEntity or destinationEntity is null.</exception>
    public void ApplyChanges(TSourceEntity? sourceEntity, TDestinationEntity? destinationEntity)
    {
      if (sourceEntity == null)
      {
        throw new ArgumentNullException(nameof(sourceEntity));
      }
      else if (destinationEntity == null)
      {
        throw new ArgumentNullException(nameof(destinationEntity));
      }
      else
      {
        foreach (var mapper in this._propertyMappers)
        {
          mapper(sourceEntity, destinationEntity);
        }
      }
    }

    /// <summary>
    /// Converts the source entity to a new instance of the destination entity.
    /// </summary>
    /// <param name="source">The source entity.</param>
    /// <returns>A new instance of the destination entity with mapped properties.</returns>
    public TDestinationEntity Convert(TSourceEntity source)
    {
      var result = new TDestinationEntity();
      ApplyChanges(source, result);
      return result;
    }
  }
}
