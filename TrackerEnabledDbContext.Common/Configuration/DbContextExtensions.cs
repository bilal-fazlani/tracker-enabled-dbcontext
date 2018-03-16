using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Configuration
{
    //https://stackoverflow.com/questions/2958921/entity-framework-4-how-to-find-the-primary-key
    internal static class DbContextExtensions
    {
        public static IEnumerable<PropertyConfiguerationKey> GetKeyNames<TEntity>(this DbContext context)
            where TEntity : class
        {
            return context.GetKeyNames(typeof(TEntity));
        }

        public static IEnumerable<PropertyConfiguerationKey> GetKeyNames(this DbContext context, Type entityType)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the mapping between CLR types and metadata OSpace
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get metadata for given CLR type
            var entityMetadata = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == entityType);

            return entityMetadata.KeyProperties.Select(x => new PropertyConfiguerationKey(x.Name, entityType.FullName));
        }
    }
}
