using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
        /// <param name="dbEntry"></param>
        /// <returns></returns>
        protected internal override EntityState StateOf(DbEntityEntry dbEntry)
        {
            if (dbEntry.State == EntityState.Unchanged)
            {
                return EntityState.Added;
            }

            return base.StateOf(dbEntry);
        }

        protected override bool IsValueChanged(string propertyName)
        {
            if (GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion)
                return true;

            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;
            object defaultValue = propertyType.DefaultValue();
            object currentValue = DbEntry.Property(propertyName).CurrentValue;

            return !propertyType.AreObjectsEqual(defaultValue, currentValue);
        }

        protected override string OriginalValue(string propertyName)
        {
            return null;
        }
    }
}
