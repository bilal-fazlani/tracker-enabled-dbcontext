using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrackerEnabledDbContext.Common.Auditors.Comparators;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Core.Common.Auditors
{
    public class DeletetionLogDetailsAuditor: ChangeLogDetailsAuditor
    {
        public DeletetionLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }

        protected override bool IsValueChanged(string propertyName)
        {
            if (GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion)
                return true;

            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;
            object defaultValue = propertyType.DefaultValue();
            object orginalvalue = OriginalValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, orginalvalue);
        }

        protected override object CurrentValue(string propertyName)
        {
            return null;
        }
    }
}
