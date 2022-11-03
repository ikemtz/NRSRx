using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public static class ContextCollectionSyncer
  {
    public static void SyncGuidCollections<TSourceEntity>(
      this IAuditableDbContext? auditableContext,
      ICollection<TSourceEntity>? sourceCollection,
      ICollection<TSourceEntity>? destinationCollection,
      Action<TSourceEntity, TSourceEntity>? updateLogic = null)
    where TSourceEntity : class, IIdentifiable<Guid>, new()
    {
      SyncCollections<TSourceEntity, TSourceEntity, Guid>(auditableContext, sourceCollection, destinationCollection, updateLogic);
    }

    public static void SyncGuidCollections<TSourceEntity, TDestinationEntity>(
     this IAuditableDbContext? auditableContext,
     ICollection<TSourceEntity>? sourceCollection,
     ICollection<TDestinationEntity>? destinationCollection,
     Action<TSourceEntity, TDestinationEntity>? updateLogic = null)
   where TSourceEntity : class, IIdentifiable<Guid>, new()
   where TDestinationEntity : class, IIdentifiable<Guid>, new()
    {
      SyncCollections<TSourceEntity, TDestinationEntity, Guid>(auditableContext, sourceCollection, destinationCollection, updateLogic);
    }
    public static void SyncIntCollections<TSourceEntity, TDestinationEntity>(
     this IAuditableDbContext? auditableContext,
     ICollection<TSourceEntity>? sourceCollection,
     ICollection<TDestinationEntity>? destinationCollection,
     Action<TSourceEntity, TDestinationEntity>? updateLogic = null)
   where TSourceEntity : class, IIdentifiable<int>, new()
   where TDestinationEntity : class, IIdentifiable<int>, new()
    {
      SyncCollections<TSourceEntity, TDestinationEntity, int>(auditableContext, sourceCollection, destinationCollection, updateLogic);
    }

    public static void SyncCollections<TSourceEntity, TDestinationEntity, TKey>(
      this IAuditableDbContext? auditableContext,
      ICollection<TSourceEntity>? sourceCollection,
      ICollection<TDestinationEntity>? destinationCollection,
      Action<TSourceEntity, TDestinationEntity>? updateLogic = null)
    where TSourceEntity : class, IIdentifiable<TKey>, new()
    where TDestinationEntity : class, IIdentifiable<TKey>, new()
    where TKey : IComparable
    {
      if (sourceCollection == null)
      {
        sourceCollection = new List<TSourceEntity>();
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

      updateLogic ??= SimpleMapper<TSourceEntity, TDestinationEntity, TKey>.Instance.ApplyChanges;

      //Delete Records from destination
      foreach (var destItem in destinationCollection.Where(src => !sourceIds.Any(t => t.Equals(src.Id))).ToList())
      {
        _ = destinationCollection?.Remove(destItem);
        _ = auditableContext.Remove(destItem);
      }
      //Updating Existing records
      foreach (var srcItem in sourceCollection.Where(src => destIds.Any(t => t.Equals(src.Id))))
      {
        var destItem = destinationCollection.First(dst => dst.Id.Equals(srcItem.Id));
        updateLogic(srcItem, destItem);
      }

      //Add New Records to destination
      foreach (var srcItem in sourceCollection.Where(src => !destIds.Any(t => t.Equals(src.Id))))
      {
        var destItem = new TDestinationEntity();
        updateLogic(srcItem, destItem);
        if (srcItem.Id.Equals(Guid.Empty))
        {
          destItem.Id = srcItem.Id;
        }
        _ = auditableContext.Add(destItem);
        destinationCollection?.Add(destItem);

      }
    }

  }
}
