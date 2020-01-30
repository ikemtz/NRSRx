using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.Models
{
  public class SimpleMapper<TEntity> : SimpleMapper<TEntity, Guid> where TEntity : IIdentifiable<Guid>
  {
  }

  public class SimpleMapper<TEntity, TIdentityType> where TEntity : IIdentifiable<TIdentityType>
  {
    private readonly string[] IgnoredProperties = new[] { "Id", "CreatedBy", "CreatedOnUtc", "UpdatedBy", "UpdatedOnUtc" };

    private readonly IEnumerable<Action<TEntity, TEntity>> _propertyMappers;
    private static SimpleMapper<TEntity, TIdentityType> _instance;
    public static SimpleMapper<TEntity, TIdentityType> Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new SimpleMapper<TEntity, TIdentityType>();
        }
        return _instance;
      }
    }
    internal SimpleMapper()
    {
      if (_propertyMappers == null)
      {
        _propertyMappers = PopulatePropertyMappings();
      }
    }

    private IEnumerable<Action<TEntity, TEntity>> PopulatePropertyMappings()
    {
      return typeof(TEntity).GetRuntimeProperties()
           .Where(prop => !IgnoredProperties.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
           .Where(prop => prop.CanWrite && prop.CanRead)
           .Select(prop => new Action<TEntity, TEntity>((src, dest) => prop.SetValue(dest, prop.GetValue(src))));
    }

    public void ApplyChanges(TEntity sourceEntity, TEntity destinationEntity)
    {
      foreach (var mapper in this._propertyMappers)
      {
        mapper(sourceEntity, destinationEntity);
      }
    }
  }
}


