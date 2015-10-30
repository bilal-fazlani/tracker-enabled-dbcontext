namespace TrackerEnabledDbContext.Common.Auditors.Comparators
{
    internal class NullableComparator : Comparator
    {
        internal override bool AreEqual(object value1, object value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null && value2 != null) return value2.Equals(value1);

            return value1.Equals(value2);
        }
    }
}