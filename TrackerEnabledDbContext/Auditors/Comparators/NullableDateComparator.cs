using System;

namespace TrackerEnabledDbContext.Common.Auditors.Comparators
{
    internal class NullableDateComparator : DateComparator
    {
        internal override bool AreEqual(object value1, object value2)
        {
            DateTime? date1 = (DateTime?)value1 ?? DateTime.MinValue;
            DateTime? date2 = (DateTime?)value2 ?? DateTime.MinValue;

            return base.AreEqual(date1, date2);
        }
    }
}