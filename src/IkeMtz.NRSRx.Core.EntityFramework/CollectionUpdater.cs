using IkeMtz.NRSRx.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class CollectionUpdater<Entity> : CollectionUpdater<Entity, Guid> where Entity : IIdentifiable<Guid>
  { }

  public class CollectionUpdater<Entity, IdentityType> where Entity : IIdentifiable<IdentityType>
  {
    private readonly static List<PropertyInfo> entityProperties = typeof(Entity).GetProperties().Where(t => !t.Name.Equals("Id")).ToList();
    public void Update(ICollection<Entity> sourceCollection, ICollection<Entity> targetCollection)
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
