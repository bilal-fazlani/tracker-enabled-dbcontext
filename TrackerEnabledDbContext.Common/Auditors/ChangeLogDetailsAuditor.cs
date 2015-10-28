using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Auditors
{
    public class ChangeLogDetailsAuditor : IDisposable
    {
        protected readonly DbEntityEntry DbEntry;
        private readonly AuditLog _log;

        public ChangeLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log)
        {
            DbEntry = dbEntry;
            _log = log;
        }

        public void Dispose()
        {
        }

        public IEnumerable<AuditLogDetail> CreateLogDetails()
        {
            Type entityType = DbEntry.Entity.GetType().GetEntityType();

            foreach (string propertyName in PropertyNamesOf(DbEntry))
            {
                if (PropertyTrackingConfiguration.IsTrackingEnabled(
                    new PropertyConfiguerationKey(propertyName, entityType.FullName), entityType ) 
                    && IsValueChanged(propertyName))
                {
                    yield return new AuditLogDetail
                    {
                        PropertyName = propertyName,
                        OriginalValue = OriginalValue(propertyName),
                        NewValue = CurrentValue(propertyName),
                        Log = _log
                    };
                }
            }
        }

        protected internal virtual EntityState StateOf(DbEntityEntry dbEntry)
        {
            return dbEntry.State;
        }

        private IEnumerable<string> PropertyNamesOf(DbEntityEntry dbEntry)
        {
            var propertyValues = (StateOf(dbEntry) == EntityState.Added)
                ? dbEntry.CurrentValues
                : dbEntry.OriginalValues;
            return propertyValues.PropertyNames;
        }

        protected virtual bool IsValueChanged(string propertyName)
        {
            var prop = DbEntry.Property(propertyName);
            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;

            var changed = (StateOf(DbEntry) == EntityState.Deleted && prop.OriginalValue != null) ||
                          (StateOf(DbEntry) == EntityState.Modified && prop.IsModified && !propertyType.AreObjectsEqual(prop.CurrentValue, prop.OriginalValue));
            return changed;
        }

        private string OriginalValue(string propertyName)
        {
            if (StateOf(DbEntry) == EntityState.Added)
            {
                return null;
            }

            var value = DbEntry.Property(propertyName).OriginalValue;
            return value?.ToString();
        }

        private string CurrentValue(string propertyName)
        {
            if (StateOf(DbEntry) == EntityState.Deleted)
            {
                // It will be invalid operation when its in deleted state. in that case, new value should be null
                return null;
            }

            var value = DbEntry.Property(propertyName).CurrentValue;
            return value?.ToString();
        }
    }
}