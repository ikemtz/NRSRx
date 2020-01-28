using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public static class ContextCollectionSyncer
  {
    public static void SyncCollections<TEntity>(this IAuditableDbContext auditableContext, IEnumerable<TEntity> sourceCollection, ICollection<TEntity> destinationCollection,
        Action<TEntity, TEntity> updateLogic = null) where TEntity : IIdentifiable
    {
      if (sourceCollection == null)
      {
        throw new ArgumentNullException(nameof(sourceCollection));
      }
      else if (destinationCollection == null)
      {
        throw new ArgumentNullException(nameof(destinationCollection));
      }
      else if (auditableContext == null)
      {
        throw new ArgumentNullException(nameof(auditableContext));
      }
      var sourceIds = sourceCollection.Select(t => t.Id).ToArray();
      var destIds = destinationCollection.Select(t => t.Id).ToArray();

      //Add New Records to destination
      foreach (var dest in sourceCollection.Where(src => !destIds.Contains(src.Id)))
      {
        destinationCollection.Add(dest);
      }

      //synchronize removed items
      foreach (var destId in destIds.Where(dstId => !sourceIds.Contains(dstId)))
      {
        var entityFrameworkBoundObject = destinationCollection.First(dst => dst.Id == destId);
        destinationCollection.Remove(entityFrameworkBoundObject);
        auditableContext.Remove(entityFrameworkBoundObject);
      }

      //If update record logic has been provided, run it
      if (updateLogic != null)
      {
        foreach (var srcItem in sourceCollection.Where(src => destIds.Contains(src.Id)))
        {
          var destItem = destinationCollection.First(dst => dst.Id == srcItem.Id);
          updateLogic(srcItem, destItem);
        }
      }
    }
  }
}
