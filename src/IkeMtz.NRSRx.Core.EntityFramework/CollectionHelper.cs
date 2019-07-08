using IkeMtz.NRSRx.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public static class CollectionHelper<Entity> where Entity : IIdentifiable
  {
    public static void SyncCollections(IEnumerable<Entity> sourceCollection, ICollection<Entity> destinationCollection,
        Action<Entity, Entity> updateLogic = null)
    {
      var sourceIds = sourceCollection.Select(t => t.Id);
      var destIds = destinationCollection.Select(t => t.Id);

      //Add New Records to destination
      sourceCollection.Where(src => !destIds.Contains(src.Id)).ToList().ForEach(destinationCollection.Add);

      //synchronize removed items
      destIds.Where(dstId => !sourceIds.Contains(dstId))
          .ToList()
          .ForEach(dstId => destinationCollection.Remove(destinationCollection.First(dst => dst.Id == dstId)));

      //If update record logic has been provided, run it
      if (updateLogic != null)
      {
        sourceCollection.Where(src => destIds.Contains(src.Id))
            .ToList()
            .ForEach(srcItem =>
        {
          var destItem = destinationCollection.First(dst => dst.Id == srcItem.Id);
          updateLogic(srcItem, destItem);
        });
      }
    }
  }
}
