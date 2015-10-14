using System.Data.Entity.ModelConfiguration;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class EntityTracker
    {
        public static TrackAllResponse<T> TrackAllProperties<T>()
        {
            OverrideTracking<T>().Enable();

            //remove all skips from propertytable
            foreach (var trackingConfiguration in TrackingDataStore.PropertyConfigStore)
            {
                if (trackingConfiguration.Key.TypeFullName == typeof(T).FullName)
                {
                    TrackingConfigurationValue removedTrackingConfigValue;
                    TrackingDataStore.PropertyConfigStore.TryRemove(trackingConfiguration.Key,
                        out removedTrackingConfigValue);
                }
            }

            return new TrackAllResponse<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>()
        {
            return new OverrideTrackingResponse<T>();
        }

        public static TrackAllResponse<T> TrackAllProperties<T>(this EntityTypeConfiguration<T> entityTypeConfig) where T: class
        {
            return TrackAllProperties<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>(this EntityTypeConfiguration<T> entityTypeConfig)
            where T : class
        {
            return OverrideTracking<T>();
        }

    }
}