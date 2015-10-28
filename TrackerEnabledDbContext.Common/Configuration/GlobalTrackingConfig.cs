namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class GlobalTrackingConfig
    {
        public static bool Enabled { get; set; } = true;

        public static bool TrackEmptyPropertiesOnAdditionAndDeletion { get; set; } = false;

        /// <summary>
        /// This will clear all the configuration done by tracking fluent API.
        /// There is usually no purpose of this method, except for unit testing.
        /// </summary>
        public static void ClearFluentConfiguration()
        {
            TrackingDataStore.EntityConfigStore.Clear();
            TrackingDataStore.PropertyConfigStore.Clear();
        }
    }
}
