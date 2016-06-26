using System.Data.Entity.Infrastructure;
using TrackingCore.Auditors.Comparators;
using TrackingCore.Configuration;
using TrackingCore.Extensions;
using TrackingCore.Models;

namespace TrackingCore.Auditors
{
    public class DeletetionLogDetailsAuditor: ChangeLogDetailsAuditor
    {
        public DeletetionLogDetailsAuditor(DbEntityEntry dbEntry) : base(dbEntry)
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
