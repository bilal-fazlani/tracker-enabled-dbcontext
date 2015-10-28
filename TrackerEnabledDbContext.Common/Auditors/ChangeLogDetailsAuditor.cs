using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Auditors
{
    public class ChangeLogDetailsAuditor : ILogDetailsAuditor
    {
        protected readonly DbEntityEntry DbEntry;
        private readonly AuditLog _log;

        public ChangeLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log)
        {
            DbEntry = dbEntry;
            _log = log;
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

            var changed = (StateOf(DbEntry) == EntityState.Modified
                && prop.IsModified && !propertyType.AreObjectsEqual(prop.CurrentValue, prop.OriginalValue));
            return changed;
        }

        protected virtual string OriginalValue(string propertyName)
        {
            var value = DbEntry.Property(propertyName).OriginalValue;
            return value?.ToString();
        }

        protected virtual string CurrentValue(string propertyName)
        {
            var value = DbEntry.Property(propertyName).CurrentValue;
            return value?.ToString();
        }
    }
}