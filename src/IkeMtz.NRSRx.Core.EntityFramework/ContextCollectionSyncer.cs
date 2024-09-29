using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Provides methods to synchronize collections within an auditable context.
  /// </summary>
  public static class ContextCollectionSyncer
  {
    /// <summary>
    /// Synchronizes collections of entities identified by GUIDs.
    /// </summary>
    /// <typeparam name="TSourceEntity">The type of the source entity.</typeparam>
    /// <typeparam name="TDestinationEntity">The type of the destination entity.</typeparam>
    /// <param name="auditableContext">The auditable context.</param>
    /// <param name="sourceCollection">The source collection.</param>
    /// <param name="destinationCollection">The destination collection.</param>
    /// <param name="updateLogic">The update logic to apply to existing entities.</param>
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

    /// <summary>
    /// Synchronizes collections of entities identified by integers.
    /// </summary>
    /// <typeparam name="TSourceEntity">The type of the source entity.</typeparam>
    /// <typeparam name="TDestinationEntity">The type of the destination entity.</typeparam>
    /// <param name="auditableContext">The auditable context.</param>
    /// <param name="sourceCollection">The source collection.</param>
    /// <param name="destinationCollection">The destination collection.</param>
    /// <param name="updateLogic">The update logic to apply to existing entities.</param>
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

    /// <summary>
    /// Synchronizes collections of entities identified by a specified key type.
    /// </summary>
    /// <typeparam name="TSourceEntity">The type of the source entity.</typeparam>
    /// <typeparam name="TDestinationEntity">The type of the destination entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="auditableContext">The auditable context.</param>
    /// <param name="sourceCollection">The source collection.</param>
    /// <param name="destinationCollection">The destination collection.</param>
    /// <param name="updateLogic">The update logic to apply to existing entities.</param>
    public static void SyncCollections<TSourceEntity, TDestinationEntity, TKey>(
      this IAuditableDbContext? auditableContext,
      ICollection<TSourceEntity>? sourceCollection,
      ICollection<TDestinationEntity>? destinationCollection,
      Action<TSourceEntity, TDestinationEntity>? updateLogic = null)
        where TSourceEntity : class, IIdentifiable<TKey>, new()
        where TDestinationEntity : class, IIdentifiable<TKey>, new()
        where TKey : IComparable
    {
      sourceCollection ??= [];
      if (destinationCollection == null)
      {
        throw new ArgumentNullException(nameof(destinationCollection));
      }
      else ArgumentNullException.ThrowIfNull(auditableContext);
      var sourceIds = sourceCollection.Select(t => t.Id).ToArray();
      var destIds = destinationCollection.Select(t => t.Id).ToArray();

      updateLogic ??= SimpleMapper<TSourceEntity, TDestinationEntity, TKey>.Instance.ApplyChanges;

      // Delete records from destination
      foreach (var destItem in destinationCollection.Where(src => !sourceIds.Any(t => t.Equals(src.Id))).ToList())
      {
        _ = destinationCollection?.Remove(destItem);
        _ = auditableContext.Remove(destItem);
      }

      // Update existing records
      foreach (var srcItem in sourceCollection.Where(src => destIds.Any(t => t.Equals(src.Id))))
      {
        var destItem = destinationCollection.First(dst => dst.Id.Equals(srcItem.Id));
        updateLogic(srcItem, destItem);
      }

      // Add new records to destination
      foreach (var srcItem in sourceCollection.Where(src => !destIds.Any(t => t.Equals(src.Id))))
      {
        var destItem = new TDestinationEntity();
        updateLogic(srcItem, destItem);
        if (srcItem.Id.Equals(Guid.Empty))
        {
          destItem.Id = srcItem.Id;
        }
        destinationCollection?.Add(destItem);
      }
    }
  }
}
