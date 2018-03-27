using System;

namespace TrackerEnabledDbContext.Common.Auditors.Comparators
{
    internal class DateComparator : Comparator
    {
        internal override bool AreEqual(object value1, object value2)
        {
            DateTime date1 = (DateTime) value1;
            DateTime date2 = (DateTime) value2;

            return date1.Year == date2.Year &&
                   date1.Month == date2.Month &&
                   date1.Day == date2.Day &&
                   date1.Hour == date2.Hour &&
                   date1.Minute == date2.Minute &&
                   date1.Second == date2.Second;
        }
    }
}