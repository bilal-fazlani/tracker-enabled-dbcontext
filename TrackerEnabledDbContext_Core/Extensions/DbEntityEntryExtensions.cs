namespace TrackerEnabledDbContext
{
    using System.Data.Entity.Infrastructure;

    public static class DbEntryExtensions
    {
        public static object GetDatabaseValue(this DbEntityEntry dbEntry, string columnName)
        {
            return dbEntry.GetDatabaseValues().GetValue<object>(columnName);
        }

        public static object GetCurrentValue(this DbEntityEntry dbEntry, string columnName)
        {
            return dbEntry.CurrentValues.GetValue<object>(columnName);
        }
    }
}
