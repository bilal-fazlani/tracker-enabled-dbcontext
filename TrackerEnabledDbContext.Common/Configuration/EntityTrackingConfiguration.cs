using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public class EntityTrackingConfiguration<T>
    {
        internal EntityTrackingConfiguration() { }

        // config chain ------------------- START

        /// <summary>
        /// If tracking is on for a given model, this method will pause tracking. 
        /// To resule tracking later, use ResumeTracking() method.
        /// </summary>
        public EntityTrackingConfiguration<T> DisableTracking()
        {
            var newvalue = new TrackingConfigurationValue(false, 
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName, 
                (key) => newvalue, 
                (key, existingValue)=> newvalue);

            return this;
        }

        /// <summary>
        /// If tracking for a given model was paused, this method will again start tracking it.
        /// Note that, if tracking was never configured for this model, calling this method will have not effect.
        /// </summary>
        public EntityTrackingConfiguration<T> EnableTracking()
        {
            var newvalue = new TrackingConfigurationValue(true, 
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName,
                (key) => newvalue, 
                (key, existingValue) => newvalue);

            return this;
        }

        public PropertyTrackingConfiguration<T> ConfigureProperties()
        {
            return new PropertyTrackingConfiguration<T>(this);
        }
 
        //config chain --------------- END

        public bool IsTrackingEnabled()
        {
            return EntityTrackingConfiguration.IsTrackingEnabled(typeof (T));
        }
    }

    public static class EntityTrackingConfiguration
    {
        internal static bool IsTrackingEnabled(Type entityType)
        {
            if (typeof (IUnTrackable).IsAssignableFrom(entityType)) return false;

            TrackingConfigurationValue value = TrackingDataStore.EntityConfigStore.GetOrAdd(
            entityType.FullName,
            (key) => EntityConfigValueFactory(key, entityType)
            );

            return value.Value;
        }

        public static EntityTrackingConfiguration<T> Configure<T>()
        {
            return new EntityTrackingConfiguration<T>();
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