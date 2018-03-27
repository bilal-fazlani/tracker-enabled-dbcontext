using System.Collections.Concurrent;

namespace TrackerEnabledDbContext.Common.Configuration
{
    internal static class TrackingDataStore
    {
        ////////////////////////// STORE /////////////////////////////

        internal static ConcurrentDictionary<string, TrackingConfigurationValue> EntityConfigStore = new ConcurrentDictionary<string, TrackingConfigurationValue>();
        internal static ConcurrentDictionary<PropertyConfigurationKey, TrackingConfigurationValue> PropertyConfigStore = new ConcurrentDictionary<PropertyConfigurationKey, TrackingConfigurationValue>();
    }
}
