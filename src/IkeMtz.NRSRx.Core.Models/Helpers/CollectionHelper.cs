using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core
{
  public static class CollectionHelper<TEntity> where TEntity : IIdentifiable
  {
    public static void SyncCollections(IEnumerable<TEntity> sourceCollection, ICollection<TEntity> destinationCollection,
        Action<TEntity, TEntity> updateLogic = null)
    {
      if (sourceCollection == null)
      {
        throw new ArgumentNullException(nameof(sourceCollection));
      }
      else if (destinationCollection == null)
      {
        throw new ArgumentNullException(nameof(destinationCollection));
      }
      var sourceIds = sourceCollection.Select(t => t.Id).ToList();
      var destIds = destinationCollection.Select(t => t.Id).ToList();

      //Add New Records to destination
      foreach (var dest in sourceCollection.Where(src => !destIds.Contains(src.Id)))
      {
        destinationCollection.Add(dest);
      }

      //synchronize removed items
      foreach (var destId in destIds.Where(dstId => !sourceIds.Contains(dstId)))
      {
        destinationCollection.Remove(destinationCollection.First(dst => dst.Id == destId));
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
