using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class EntityTrackingConfiguration<T>
    {
        /// <summary>
        /// If tracking is on for a given model, this method will pause tracking. 
        /// To resule tracking later, use ResumeTracking() method.
        /// </summary>
        public static void PauseTracking()
        {
            var newvalue = new TrackingConfigurationValue(false, 
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName, 
                (key) => newvalue, 
                (key, existingValue)=> newvalue);
        }

        /// <summary>
        /// If tracking for a given model was paused, this method will again start tracking it.
        /// Note that, if tracking was never configured for this model, calling this method will have not effect.
        /// </summary>
        public static void StartTracking()
        {
            var newvalue = new TrackingConfigurationValue(true, 
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName,
                (key) => newvalue, 
                (key, existingValue) => newvalue);
        }

        public static bool IsTrackingEnabled()
        {
            TrackingConfigurationValue value = TrackingDataStore.EntityConfigStore.GetOrAdd(
                typeof (T).FullName,
                (key)=> EntityTrackingConfiguration.EntityConfigValueFactory(key, typeof(T)));

            return value.Value;
        }
    }

    public static class EntityTrackingConfiguration
    {
        public static bool IsTrackingEnabled(Type entityType)
        {
            TrackingConfigurationValue value = TrackingDataStore.EntityConfigStore.GetOrAdd(
                entityType.FullName,
                (key) => EntityConfigValueFactory(key, entityType)
                );

            return value.Value;
        }

        internal static TrackingConfigurationValue EntityConfigValueFactory(string key, Type entityType)
        {
            TrackChangesAttribute trackChangesAttribute =
                entityType.GetCustomAttributes(true).OfType<TrackChangesAttribute>().SingleOrDefault();
            bool value = trackChangesAttribute != null && trackChangesAttribute.Enabled;

            return new TrackingConfigurationValue(value);
        }
    }
}