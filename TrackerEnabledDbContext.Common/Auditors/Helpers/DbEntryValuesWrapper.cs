using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.Common.Auditors.Helpers
{
    public class DbEntryValuesWrapper
    {
        protected readonly DbEntityEntry _dbEntry;
        private DbPropertyValues _entryValues = null;

        private DbPropertyValues EntryPropertyValues => _entryValues ?? (_entryValues = _dbEntry.GetDatabaseValues());

        public DbEntryValuesWrapper(DbEntityEntry dbEntry)
        {
            _dbEntry = dbEntry;
        }

        public object OriginalValue(string propertyName)
        {
            object originalValue = null;

            if (GlobalTrackingConfig.DisconnectedContext)
            {
                originalValue = EntryPropertyValues.GetValue<object>(propertyName);
            }
            else
            {
                originalValue = _dbEntry.Property(propertyName).OriginalValue;
            }

            return originalValue;
        }
    }
}
