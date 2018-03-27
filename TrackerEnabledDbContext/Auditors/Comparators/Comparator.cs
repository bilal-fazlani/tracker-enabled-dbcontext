namespace TrackerEnabledDbContext.Common.Auditors.Comparators
{
    internal class Comparator
    {
        internal virtual bool AreEqual(object value1, object value2)
        {
            return value1 == value2;
        }
    }
}
