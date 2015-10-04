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
        private readonly DbEntityEntry _dbEntry;
        private readonly AuditLog _log;

        public ChangeLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log)
        {
            _dbEntry = dbEntry;
            _log = log;
        }

        public void Dispose()
        {
        }

        public IEnumerable<AuditLogDetail> CreateLogDetails()
        {
            Type entityType = _dbEntry.Entity.GetType().GetEntityType();

            foreach (string propertyName in PropertyNamesOf(_dbEntry))
            {
                if (PropertyTrackingConfiguration.IsTrackingEnabled(new PropertyConfiguerationKey(propertyName, entityType.FullName), entityType ) && IsValueChanged(propertyName))
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

        private bool IsValueChanged(string propertyName)
        {
            var prop = _dbEntry.Property(propertyName);
            var changed = (StateOf(_dbEntry) == EntityState.Added && prop.CurrentValue != null) ||
                          (StateOf(_dbEntry) == EntityState.Deleted && prop.OriginalValue != null) ||
                          (StateOf(_dbEntry) == EntityState.Modified && prop.IsModified);
            return changed;
        }

        private string OriginalValue(string propertyName)
        {
            if (StateOf(_dbEntry) == EntityState.Added)
            {
                return null;
            }

            var value = _dbEntry.Property(propertyName).OriginalValue;
            return value?.ToString();
        }

        private string CurrentValue(string propertyName)
        {
            if (StateOf(_dbEntry) == EntityState.Deleted)
            {
                // It will be invalid operation when its in deleted state. in that case, new value should be null
                return null;
            }

            var value = _dbEntry.Property(propertyName).CurrentValue;
            return value?.ToString();
        }
    }
}