using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class DbEntryExtensions
    {
        public static object GetPrimaryKeyValues(
            this DbEntityEntry dbEntry, 
            List<PropertyConfiguerationKey> properties)
        {
            if (properties.Count == 1)
            {
                return dbEntry.GetDatabaseValues().GetValue<object>(properties.Select(x=>x.PropertyName).First());
            }
            if (properties.Count > 1)
            {
                string output = "[";

                output += string.Join(",",
                    properties.Select(colName => dbEntry.GetDatabaseValues().GetValue<object>(colName.PropertyName)));

                output += "]";
                return output;
            }
            throw new KeyNotFoundException("key not found for " + dbEntry.Entity.GetType().FullName);
        }
    }
}