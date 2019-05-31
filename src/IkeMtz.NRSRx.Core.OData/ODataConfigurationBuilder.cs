using IkeMtz.NRSRx.Core.Models;
using Microsoft.AspNet.OData.Builder;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace IkeMtz.NRSRx.Core.OData
{
    public class ODataConfigurationBuilder<Entity>
      : ODataConfigurationBuilder<Entity, Guid> where Entity : class, IIdentifiable<Guid>
    {
    }

    public class ODataConfigurationBuilder<Entity, IdentifiableType> where Entity : class, IIdentifiable<IdentifiableType>
    {
        private const string DATE_SUFFIX = "Date";
        public static EntitySetConfiguration<Entity> EntitySetBuilder(ODataModelBuilder builder, string setName = null)
        {
            if (string.IsNullOrWhiteSpace(setName))
            {
                // Need to pluralize the entity name
                setName = $"{typeof(Entity).Name}s";
            }

            var set = builder.EntitySet<Entity>(setName);
            TransformEntityEdmDates(set.EntityType);
            return set;
        }

        private static void TransformEntityEdmDates(EntityTypeConfiguration<Entity> config)
        {
            var entityParam = Expression.Parameter(typeof(Entity), nameof(Entity).ToLower());
            var entityProperties = typeof(Entity).GetProperties()
                .Where(propertyInfo => propertyInfo.Name.EndsWith(DATE_SUFFIX));

            var dateType = typeof(DateTime);
            entityProperties.Where(propertyInfo => dateType == propertyInfo.PropertyType)
               .ToList()
               .ForEach(propertyInfo =>
               {
                   var propertyExpression = Expression.Property(entityParam, propertyInfo);
                   var dateLambda = Expression.Lambda<Func<Entity, DateTime>>(propertyExpression, new[] { entityParam });
                   config.Property(dateLambda).AsDate();
               });

            dateType = typeof(DateTime?);
            entityProperties.Where(propertyInfo => dateType == propertyInfo.PropertyType)
            .ToList()
            .ForEach(propertyInfo =>
            {
                var propertyExpression = Expression.Property(entityParam, propertyInfo);
                var dateLambda = Expression.Lambda<Func<Entity, DateTime?>>(propertyExpression, new[] { entityParam });
                config.Property(dateLambda).AsDate();
            });
        }
    }
}
