using System;
using System.Reflection;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class EntityTracker
    {
        public static TrackAllResponse<T> TrackAllProperties<T>()
        {
            OverrideTracking<T>().Enable();

            var allPublicInstanceProperties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //add high priority tracking to all properties
            foreach (var property in allPublicInstanceProperties)
            {
                Func<PropertyConfiguerationKey, TrackingConfigurationValue, TrackingConfigurationValue> factory =
                    (key,value) => new TrackingConfigurationValue(true, TrackingConfigurationPriority.High);

                TrackingDataStore.PropertyConfigStore.AddOrUpdate(
                    new PropertyConfiguerationKey(property.Name, typeof (T).FullName),
                    new TrackingConfigurationValue(true, TrackingConfigurationPriority.High),
                    factory
                    );
            }

            return new TrackAllResponse<T>();
        }

        public static OverrideTrackingResponse<T> OverrideTracking<T>()
        {
            return new OverrideTrackingResponse<T>();
        }
    }
}