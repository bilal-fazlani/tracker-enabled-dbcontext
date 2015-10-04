using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TrackerEnabledDbContext.Common.Interfaces;

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
        public static PropertyTrackingConfiguration<T> StartTracking()
        {
            var newvalue = new TrackingConfigurationValue(true, 
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName,
                (key) => newvalue, 
                (key, existingValue) => newvalue);

            return new PropertyTrackingConfiguration<T>();
        }

        public static bool IsTrackingEnabled()
        {
            return EntityTrackingConfiguration.IsTrackingEnabled(typeof (T));
        }
    }

    public static class EntityTrackingConfiguration
    {
        public static bool IsTrackingEnabled(Type entityType)
        {
            if (typeof (IUnTrackable).IsAssignableFrom(entityType)) return false;

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
            bool value = trackChangesAttribute != null;

            return new TrackingConfigurationValue(value);
        }
    }
}