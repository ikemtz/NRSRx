using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public static class ContextCollectionSyncer
  {
    public static void SyncCollections<TEntity>(this IAuditableDbContext auditableContext, IEnumerable<TEntity> sourceCollection, IEnumerable<TEntity> destinationCollection,
        Action<TEntity, TEntity>? updateLogic = null) where TEntity : class, IIdentifiable, new()
    {
      if (sourceCollection == null)
      {
        return;
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
      var destCollectiion = (destinationCollection as IList<TEntity>);
      //Add New Records to destination
      sourceCollection.Where(src => !destIds.Contains(src.Id)).ToList().ForEach(item =>
      {
        _ = auditableContext.Add(item);
        destCollectiion?.Add(item);
      });

      //synchronize removed items
      foreach (var destId in destIds.Where(dstId => !sourceIds.Contains(dstId)))
      {
        var entityFrameworkBoundObject = destCollectiion.First(dst => dst.Id == destId);
        _ = destCollectiion?.Remove(entityFrameworkBoundObject);
        _ = auditableContext.Remove(entityFrameworkBoundObject);
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
      else
      {
        foreach (var srcItem in sourceCollection.Where(src => destIds.Contains(src.Id)))
        {
          var destItem = destinationCollection.First(dst => dst.Id == srcItem.Id);
          SimpleMapper<TEntity>.Instance.ApplyChanges(srcItem, destItem);
        }
      }
    }

    public static void SyncCollections<TSourceEntity, TDestinationEntity>(this IAuditableDbContext? auditableContext, IEnumerable<TSourceEntity>? sourceCollection, IEnumerable<TDestinationEntity>? destinationCollection,
    Action<TSourceEntity, TDestinationEntity>? updateLogic = null) where TSourceEntity : class, IInsertableDto, new()
      where TDestinationEntity : class, IIdentifiable, new()
    {
      if (sourceCollection == null)
      {
        return;
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
      var destCollectiion = (destinationCollection as IList<TDestinationEntity>);

      updateLogic ??= SimpleMapper<TSourceEntity, TDestinationEntity>.Instance.ApplyChanges;
      //Updating Existing records
      foreach (var srcItem in sourceCollection.Where(src => destIds.Any(t => t == src.Id.Value)))
      {
        var destItem = destinationCollection.First(dst => dst.Id == srcItem.Id);
        updateLogic(srcItem, destItem);
      }

      //Create New Records to destination
      foreach (var srcItem in sourceCollection.Where(src => !destIds.Any(t => t == src.Id.Value)))
      {
        var destItem = new TDestinationEntity();
        updateLogic(srcItem, destItem);
        if (srcItem.Id.GetValueOrDefault() != Guid.Empty)
        {
          destItem.Id = srcItem.Id.Value;
        }
        _ = auditableContext.Add(destItem);
        destCollectiion?.Add(destItem);

      }

      //Delete Records from destination
      foreach (var destItem in destinationCollection.Where(src => !sourceIds.Any(t => t.Value == src.Id)).ToList())
      {
        _ = destCollectiion?.Remove(destItem);
        _ = auditableContext.Remove(destItem);
      }
    }
  }
}
