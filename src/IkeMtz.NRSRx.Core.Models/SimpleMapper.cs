using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Models
{
  internal static class SimpleMapperHelper
  {
    internal static readonly string[] IgnoredProperties = { "Id", "CreatedBy", "CreatedOnUtc", "UpdatedBy", "UpdatedOnUtc" };
    internal static readonly string[] IgnoredInterfaces = { nameof(IIdentifiable), typeof(IIdentifiable<>).Name, typeof(ICollection<>).Name };
  }

  public class SimpleMapper<TSourceEntity, TDestinationEntity> :
    SimpleMapper<TSourceEntity, TDestinationEntity, Guid>
    where TSourceEntity : class
    where TDestinationEntity : class, IIdentifiable<Guid>, new()
  {
  }

  public class SimpleMapper<TEntity> : SimpleMapper<TEntity, TEntity, Guid>
    where TEntity : class, IIdentifiable<Guid>, new()
  {
    internal SimpleMapper()
    {
    }
    protected override IEnumerable<Action<TEntity, TEntity>> PopulatePropertyMappings()
    {
      return FilteredSourceProperties
        .Where(prop => prop.CanWrite && prop.CanRead)
        .Select(prop => new Action<TEntity, TEntity>((src, dest) =>
          prop.SetValue(dest, prop.GetValue(src))));
    }
  }

  public class SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType>
    where TIdentityType : IComparable
    where TSourceEntity : class
    where TDestinationEntity : class, IIdentifiable<TIdentityType>, new()
  {
    private IEnumerable<Action<TSourceEntity, TDestinationEntity>> _propertyMappers;
    private static SimpleMapper<TSourceEntity, TDestinationEntity, TIdentityType> _instance;
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
    protected virtual IEnumerable<PropertyInfo> FilteredSourceProperties => typeof(TSourceEntity).GetRuntimeProperties()
        .Where(prop => !SimpleMapperHelper.IgnoredProperties.Contains(prop.Name, StringComparer.CurrentCulture))
        .Where(prop => !prop.PropertyType.GetTypeInfo().GetInterfaces().Select(t => t.Name).Any(t =>
          SimpleMapperHelper.IgnoredInterfaces.Contains(t, StringComparer.CurrentCulture)));

    protected virtual IEnumerable<Action<TSourceEntity, TDestinationEntity>> PopulatePropertyMappings()
    {
      var sourceProperties = FilteredSourceProperties
          .Where(prop => prop.CanRead)
          .ToDictionary(prop => prop.Name);
      return typeof(TDestinationEntity).GetRuntimeProperties()
            .Where(prop => sourceProperties.Keys.Contains(prop.Name, StringComparer.CurrentCulture))
            .Where(prop => prop.CanWrite)
            .Select(prop => new Action<TSourceEntity, TDestinationEntity>((src, dest) =>
              prop.SetValue(dest, sourceProperties[prop.Name].GetValue(src))
            ));
    }

    public void ApplyChanges(TSourceEntity sourceEntity, TDestinationEntity destinationEntity)
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

    public TDestinationEntity Convert(TSourceEntity source)
    {
      var result = new TDestinationEntity();
      ApplyChanges(source, result);
      return result;
    }
  }
}
