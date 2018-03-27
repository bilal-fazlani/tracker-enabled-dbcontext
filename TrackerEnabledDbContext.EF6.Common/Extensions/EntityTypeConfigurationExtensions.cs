using System.Data.Entity.ModelConfiguration;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.EF6.Common.Extensions
{
    public static class EntityTypeConfigurationExtensions
    {
        public static TrackAllResponse<T> TrackAllProperties<T>(this EntityTypeConfiguration<T> entityTypeConfig) where T : class
        {
            return EntityTracker.TrackAllProperties<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>(this EntityTypeConfiguration<T> entityTypeConfig)
            where T : class
        {
            return EntityTracker.OverrideTracking<T>();
        }
    }
}
