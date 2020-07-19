using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.AspNet.OData.Builder;

namespace IkeMtz.NRSRx.Core.OData
{
  public class ODataConfigurationBuilder<TEntity>
    : ODataConfigurationBuilder<TEntity, Guid> where TEntity : class, IIdentifiable<Guid>
  {
  }

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
#pragma warning disable S1118 // Utility classes should not have public constructors
  public class ODataConfigurationBuilder<TEntity, TIdentifiableType>
    where TIdentifiableType : IComparable
    where TEntity : class, IIdentifiable<TIdentifiableType>
#pragma warning restore S1118 // Utility classes should not have public constructors
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
  {
    private const string DATE_SUFFIX = "Date";
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static EntitySetConfiguration<TEntity> EntitySetBuilder(ODataModelBuilder builder, string setName = null)
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }
      if (string.IsNullOrWhiteSpace(setName))
      {
        // Need to pluralize the entity name
        setName = $"{typeof(TEntity).Name}s";
      }

      var set = builder.EntitySet<TEntity>(setName);
      TransformEntityEdmDates(set.EntityType);
      return set;
    }

    private static void TransformEntityEdmDates(EntityTypeConfiguration<TEntity> config)
    {
      var entityParam = Expression.Parameter(typeof(TEntity), nameof(TEntity).ToLower(CultureInfo.CurrentCulture));
      var entityProperties = typeof(TEntity).GetProperties()
          .Where(propertyInfo => propertyInfo.Name.EndsWith(DATE_SUFFIX, StringComparison.CurrentCultureIgnoreCase));

      var dateType = typeof(DateTime);
      entityProperties.Where(propertyInfo => dateType == propertyInfo.PropertyType)
         .ToList()
         .ForEach(propertyInfo =>
         {
           var propertyExpression = Expression.Property(entityParam, propertyInfo);
           var dateLambda = Expression.Lambda<Func<TEntity, DateTime>>(propertyExpression, new[] { entityParam });
           _ = config.Property(dateLambda).AsDate();
         });

      dateType = typeof(DateTime?);
      entityProperties.Where(propertyInfo => dateType == propertyInfo.PropertyType)
      .ToList()
      .ForEach(propertyInfo =>
      {
        var propertyExpression = Expression.Property(entityParam, propertyInfo);
        var dateLambda = Expression.Lambda<Func<TEntity, DateTime?>>(propertyExpression, new[] { entityParam });
        _ = config.Property(dateLambda).AsDate();
      });
    }
  }
}
