using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class DbEntryExtensions
    {
        public static object GetDatabaseValue(this DbEntityEntry dbEntry, string columnName)
        {
            return dbEntry.GetDatabaseValues().GetValue<object>(columnName);
        }

        public static object GetPrimaryKeyValues(this DbEntityEntry dbEntry, IEnumerable<string> columnNames)
        {
            if (columnNames.Count() == 1)
            {
                return dbEntry.GetDatabaseValues().GetValue<object>(columnNames.First());
            }
            if (columnNames.Count() > 1)
            {
                string output = "[";

                output += string.Join(",",
                    columnNames.Select(colName => { return dbEntry.GetDatabaseValues().GetValue<object>(colName); }));

                output += "]";
                return output;
            }
            throw new KeyNotFoundException("key not found for " + dbEntry.Entity.GetType().FullName);
        }

        public static object GetCurrentValue(this DbEntityEntry dbEntry, string columnName)
        {
            return dbEntry.CurrentValues.GetValue<object>(columnName);
        }
    }
}