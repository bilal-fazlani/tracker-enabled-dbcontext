using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrackerEnabledDbContext.Common.Auditors.Comparators;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Core.Common.Auditors
{
    public class ChangeLogDetailsAuditor : ILogDetailsAuditor
    {
        protected readonly EntityEntry DbEntry;
        private readonly AuditLog _log;

        public ChangeLogDetailsAuditor(EntityEntry dbEntry, AuditLog log)
        {
            DbEntry = dbEntry;
            _log = log;
        }

        public IEnumerable<AuditLogDetail> CreateLogDetails()
        {
            Type entityType = DbEntry.Entity.GetType();            

            //not using PropertyNamesOfEntity
            foreach (MemberEntry me in DbEntry.Members)
            {
                string pn = me.Metadata.Name;

                //skip anything with SkipTracking attribute applied and (NavigationEntry and ReferenceEntry)
                if (PropertyTrackingConfiguration.IsTrackingEnabled(new PropertyConfigurationKey(pn, entityType.FullName), entityType) && (IsPropertyEntry(pn) && IsValueChanged(pn)))
                {
                    yield return new AuditLogDetail
                    {
                        PropertyName = pn,
                        OriginalValue = OriginalValue(pn)?.ToString(),
                        NewValue = CurrentValue(pn)?.ToString(),
                        Log = _log
                    };
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
            return propertyValues.Properties.Select(n => n.Name);
        }

        protected virtual bool IsValueChanged(string propertyName)
        {
            var prop = DbEntry.Property(propertyName);
            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;

            object originalValue = OriginalValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            var changed = (StateOfEntity() == EntityState.Modified && prop.IsModified && !comparator.AreEqual(CurrentValue(propertyName), originalValue));
            return changed;
        }

        protected virtual object OriginalValue(string propertyName)
        {
            object originalValue = null;

            //if (GlobalTrackingConfig.DisconnectedContext)
            //{
                originalValue = DbEntry.GetDatabaseValues().GetValue<object>(propertyName);
            //}
            //else
            //{
            //    originalValue = DbEntry.Property(propertyName).OriginalValue;
            //}

            return originalValue;
        }

        protected virtual object CurrentValue(string propertyName)
        {
            var value = DbEntry.Property(propertyName).CurrentValue;
            return value;
        }

        private bool IsPropertyEntry(string pn)
        {
            PropertyEntry entryMember = DbEntry.Member(pn) as PropertyEntry;
            return entryMember != null;
        }
    }
}