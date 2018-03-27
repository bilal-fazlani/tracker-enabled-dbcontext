using System;

namespace TrackerEnabledDbContext.Common.Auditors.Comparators
{
    internal class StringComparator : Comparator
    {
        internal override bool AreEqual(object value1, object value2)
        {
            return String.CompareOrdinal(Convert.ToString(value1), Convert.ToString(value2)) == 0;
        }
    }
}