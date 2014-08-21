using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
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
            foreach (string propertyName in _dbEntry.OriginalValues.PropertyNames)
            {
                if (IsTrackingEnabled(propertyName) && IsValueChanged(propertyName))
                {
                    yield return new AuditLogDetail
                    {
                        ColumnName = GetColumnName(propertyName),
                        OrginalValue = OriginalValue(propertyName),
                        NewValue = CurrentValue(propertyName),
                        Log = _log
                    };
                }
            }
        }

        private bool IsTrackingEnabled(string propertyName)
        {
            var entityType = Helper.GetEntityType(_dbEntry.Entity.GetType());
            var skipTracking = entityType.GetProperty(propertyName).GetCustomAttributes(false).OfType<SkipTracking>().SingleOrDefault();
            return skipTracking == null || !skipTracking.Enabled;
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
                originalValue = _dbEntry.GetDatabaseValues().GetValue<object>(propertyName) == null ? null : _dbEntry.GetDatabaseValues().GetValue<object>(propertyName).ToString();
            }

            return originalValue;
        }

        private string CurrentValue(string propertyName)
        {
            string newValue;

            try
            {
                newValue = _dbEntry.CurrentValues.GetValue<object>(propertyName) == null
                    ? null
                    : _dbEntry.CurrentValues.GetValue<object>(propertyName).ToString();
            }
            catch (InvalidOperationException) // It will be invalid operation when its in deleted state. in that case, new value should be null
            {
                newValue = null;
            }

            return newValue;
        }

        private string GetColumnName(string propertyName)
        {
            string columnName = propertyName;

            var entityType = Helper.GetEntityType(_dbEntry.Entity.GetType());
            var columnAttribute = entityType.GetProperty(propertyName).GetCustomAttribute<ColumnAttribute>(false);
            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
            {
                columnName = columnAttribute.Name;
            }
            return columnName;
        }

        public void Dispose()
        {
            
        }
    }
}
