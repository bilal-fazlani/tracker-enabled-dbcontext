using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Auditors.Comparators;
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

            foreach (string propertyName in PropertyNamesOfEntity())
            {
                if (PropertyTrackingConfiguration.IsTrackingEnabled(
                    new PropertyConfiguerationKey(propertyName, entityType.FullName), entityType)
                    && IsValueChanged(propertyName))
                {
                    if (IsComplexType(propertyName))
                    {
                        foreach (var auditLogDetail in CreateComplexTypeLogDetails(propertyName))
                        {
                            yield return auditLogDetail;
                        }
                    }
                    else
                    {
                        yield return new AuditLogDetail
                        {
                            PropertyName = propertyName,
                            OriginalValue = OriginalValue(propertyName)?.ToString(),
                            NewValue = CurrentValue(propertyName)?.ToString(),
                            Log = _log
                        };
                    }
                }
            }
        }

        protected internal virtual EntityState StateOfEntity()
        {
            return DbEntry.State;
        }

        private IEnumerable<string> PropertyNamesOfEntity()
        {
            var propertyValues = (StateOfEntity() == EntityState.Added)
                ? DbEntry.CurrentValues
                : DbEntry.OriginalValues;
            return propertyValues.PropertyNames;
        }

        protected virtual bool IsValueChanged(string propertyName)
        {
            var prop = DbEntry.Property(propertyName);
            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;

            object originalValue = OriginalValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            var changed = (StateOfEntity() == EntityState.Modified
                && prop.IsModified && !comparator.AreEqual(CurrentValue(propertyName), originalValue));
            return changed;
        }

        protected virtual object OriginalValue(string propertyName)
        {
            object originalValue = null;

            if (GlobalTrackingConfig.DisconnectedContext)
            {
                originalValue = DbEntry.GetDatabaseValues().GetValue<object>(propertyName);
            }
            else
            {
                originalValue = DbEntry.Property(propertyName).OriginalValue;
            }

            return originalValue;
        }

        protected virtual object CurrentValue(string propertyName)
        {
            var value = DbEntry.Property(propertyName).CurrentValue;
            return value;
        }

        private bool IsComplexType(string propertyName)
        {
            var entryMember = DbEntry.Member(propertyName) as DbComplexPropertyEntry;

            return entryMember != null;
        }

        private IEnumerable<AuditLogDetail> CreateComplexTypeLogDetails(string propertyName)
        {
            var entryMember = DbEntry.Member(propertyName) as DbComplexPropertyEntry;

            if (entryMember != null)
            {
                var complexTypeObj = entryMember.CurrentValue.GetType();

                foreach (var pi in complexTypeObj.GetProperties())
                {
                    var complexTypePropertyName = $"{propertyName}_{pi.Name}";
                    var complexTypeOrigValue = OriginalValue(propertyName);
                    var complexTypeNewValue = CurrentValue(propertyName);

                    var origValue = complexTypeOrigValue == null ? null : pi.GetValue(complexTypeOrigValue);
                    var newValue = complexTypeNewValue == null ? null : pi.GetValue(complexTypeNewValue);

                    Comparator comparator = ComparatorFactory.GetComparator(complexTypeObj);

                    if (!comparator.AreEqual(newValue, origValue))
                    {
                        yield return new AuditLogDetail
                        {
                            PropertyName = complexTypePropertyName,
                            OriginalValue = origValue?.ToString(),
                            NewValue = newValue?.ToString(),
                            Log = _log
                        };
                    }
                }
            }
        }

    }
}