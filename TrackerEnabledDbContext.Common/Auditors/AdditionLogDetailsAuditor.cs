using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Auditors.Comparators;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Auditors
{
    /// <summary>
    /// Creates AuditLogDetails for entries added in a previous call to SaveChanges.
    /// </summary>
    public class AdditionLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public AdditionLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }

        /// <summary>
        /// Treat unchanged entries as added entries when creating audit records.
        /// </summary>
        /// <returns></returns>
        protected internal override EntityState StateOfEntity()
        {
            if (DbEntry.State == EntityState.Unchanged)
            {
                return EntityState.Added;
            }

            return base.StateOfEntity();
        }

        protected override bool IsValueChanged(string propertyName)
        {
            if (GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion)
                return true;

            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;
            object defaultValue = propertyType.DefaultValue();
            object currentValue = CurrentValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, currentValue);
        }

        protected override object OriginalValue(string propertyName)
        {
            return null;
        }
    }
}
