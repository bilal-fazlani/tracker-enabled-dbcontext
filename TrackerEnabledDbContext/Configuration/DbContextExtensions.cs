using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.Core.Common.Configuration
{
    //https://stackoverflow.com/questions/30688909/how-to-get-primary-key-value-with-entity-framework-core
    internal static class DbContextExtensions
    {
        public static IEnumerable<PropertyConfigurationKey> GetKeyNames<TEntity>(this DbContext context)
            where TEntity : class
        {
            return context.GetKeyNames(typeof(TEntity));            
        }

        public static IEnumerable<PropertyConfigurationKey> GetKeyNames(this DbContext context, Type entityType)
        {
            return context.Model.FindEntityType(entityType).FindPrimaryKey().Properties.Select(x => new PropertyConfigurationKey(x.Name, entityType.FullName));
        }
    }
}
