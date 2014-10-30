using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Models;

namespace TrackerEnabledDbContext
{
    public class LogDetailsAuditor : IDisposable
    {
        private readonly DbEntityEntry _dbEntry;
        private readonly AuditLog _log;

        public LogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log)
        {
            _dbEntry = dbEntry;
            _log = log;
        }

        public IEnumerable<AuditLogDetail> GetLogDetails()
        {
            var type = _dbEntry.Entity.GetType().GetEntityType();

            foreach (var propertyName in _dbEntry.OriginalValues.PropertyNames)
            {
                if (type.IsTrackingEnabled(propertyName) && IsValueChanged(propertyName))
                {
                    yield return new AuditLogDetail
                    {
                        ColumnName = type.GetColumnName(propertyName),
                        OrginalValue = OriginalValue(propertyName),
                        NewValue = CurrentValue(propertyName),
                        Log = _log
                    };
                }
            }
        }

        private bool IsValueChanged(string propertyName)
        {
            return !Equals(OriginalValue(propertyName), CurrentValue(propertyName));
        }

        private string OriginalValue(string propertyName)
        {
            string originalValue;

            if (_dbEntry.State == EntityState.Unchanged)
            {
                originalValue = null;
            }
            else
            {
                var value = _dbEntry.GetDatabaseValue(propertyName);
                originalValue = (value != null) ? value.ToString() : null;
            }

            return originalValue;
        }

        private string CurrentValue(string propertyName)
        {
            string newValue;

            try
            {
                var value = _dbEntry.GetCurrentValue(propertyName);
                newValue = (value != null) ? value.ToString() : null;
            }
            catch (InvalidOperationException) // It will be invalid operation when its in deleted state. in that case, new value should be null
            {
                newValue = null;
            }

            return newValue;
        }

        public void Dispose()
        {
            
        }
    }
}
