using Microsoft.EntityFrameworkCore;
using TrackerEnabledDbContext.Common.Configuration;

namespace TrackerEnabledDbContext.Core.Common.Extensions
{
    public static class EntityTypeConfigurationExtensions
    {
        public static TrackAllResponse<T> TrackAllProperties<T>(this IEntityTypeConfiguration<T> entityTypeConfig) where T : class
        {
            return EntityTracker.TrackAllProperties<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>(this IEntityTypeConfiguration<T> entityTypeConfig)
            where T : class
        {
            return EntityTracker.OverrideTracking<T>();
        }
    }
}
