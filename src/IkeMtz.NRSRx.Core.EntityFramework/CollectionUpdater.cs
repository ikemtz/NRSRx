using IkeMtz.NRSRx.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class CollectionUpdater<TEntity> : CollectionUpdater<TEntity, Guid> where TEntity : IIdentifiable<Guid>
  { }

  public class CollectionUpdater<TEntity, TIdentityType> where TEntity : IIdentifiable<TIdentityType>
  {
    private readonly static List<PropertyInfo> entityProperties = typeof(TEntity).GetProperties().Where(t => !t.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && t.CanWrite).ToList();
    public void Update(ICollection<TEntity> sourceCollection, ICollection<TEntity> targetCollection)
    {
      sourceCollection.ToList().ForEach(sourceEntity =>
      {
        var targetEntity = targetCollection.FirstOrDefault(tc => tc.Id.Equals(sourceEntity.Id));
        if (targetEntity == null)
        {
          targetCollection.Add(targetEntity);
        }
        else
        {
          entityProperties.ForEach(prop =>
          {
            prop.SetValue(targetEntity, prop.GetValue(sourceEntity));
          });
        }
      });
      targetCollection.ToList().ForEach(targetEntity =>
      {
        if (!sourceCollection.Any(sc => sc.Id.Equals(targetEntity.Id)))
        {
          targetCollection.Remove(targetEntity);
        }
      });
    }
  }
}
