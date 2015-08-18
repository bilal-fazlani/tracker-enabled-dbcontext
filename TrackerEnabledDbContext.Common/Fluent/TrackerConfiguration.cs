using TrackerEnabledDbContext.Common.Extensions;

namespace TrackerEnabledDbContext.Common.Fluent
{
    public static class TrackerConfiguration<T> where T : class
    {
        public static TrackerColumnConfigurator<T> EnableTableTracking()
        {
            TypeExtensions.EnableTableTracking<T>();
            return new TrackerColumnConfigurator<T>();
        }
    }
}