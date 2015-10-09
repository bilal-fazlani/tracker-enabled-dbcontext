using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
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

        internal static TrackingConfigurationValue EntityConfigValueFactory(string key, Type entityType)
        {
            TrackChangesAttribute trackChangesAttribute =
                entityType.GetCustomAttributes(true).OfType<TrackChangesAttribute>().SingleOrDefault();
            bool value = trackChangesAttribute != null;

            return new TrackingConfigurationValue(value);
        }

        public static TrackAllResponse<T> TrackAllProperties<T>()
        {
            OverrideTracking<T>().Enable();

            //remove all skips from propertytable
            foreach (var trackingConfiguration in TrackingDataStore.PropertyConfigStore)
            {
                if (trackingConfiguration.Key.TypeFullName == typeof (T).FullName)
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
    }

    public class OverrideTrackingResponse<T>
    {
        public OverrideTrackingResponse<T> Enable()
        {
            //insert/update in entity table
            var newvalue = new TrackingConfigurationValue(true,
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName,
                (key) => newvalue,
                (key, existingValue) => newvalue);

            return this;
        }

        public OverrideTrackingResponse<T> Disable()
        {
            var newvalue = new TrackingConfigurationValue(false,
                TrackingConfigurationPriority.High);

            TrackingDataStore.EntityConfigStore.AddOrUpdate(
                typeof(T).FullName,
                (key) => newvalue,
                (key, existingValue) => newvalue);

            return this;
        }

        public OverrideTrackingResponse<T> Enable(Expression<Func<T, object>> property)
        {
            TrackProperty(property);
            return this;
        }

        public OverrideTrackingResponse<T> Disable(Expression<Func<T, object>> property)
        {
            SkipProperty(property);
            return this;
        }

        internal static void SkipProperty(Expression<Func<T, object>> property)
        {
            PropertyInfo info = property.GetPropertyInfo();
            var newValue = new TrackingConfigurationValue(false, TrackingConfigurationPriority.High);

            TrackingDataStore.PropertyConfigStore.AddOrUpdate(
                new PropertyConfiguerationKey(info.Name, info.DeclaringType.FullName),
                newValue,
                (existingKey, existingvalue) => newValue);
        }

        internal static void TrackProperty(Expression<Func<T, object>> property)
        {
            PropertyInfo info = property.GetPropertyInfo();
            var newValue = new TrackingConfigurationValue(true, TrackingConfigurationPriority.High);
            TrackingDataStore.PropertyConfigStore.AddOrUpdate(
                new PropertyConfiguerationKey(info.Name, info.DeclaringType.FullName),
                newValue,
                (key, value) => newValue);
        }

    }

    public class TrackAllResponse<T>
    {
        public ExceptResponse<T> Except(Expression<Func<T, object>> property)
        {
            OverrideTrackingResponse<T>.SkipProperty(property);
            return new ExceptResponse<T>();
        }
    }

    public class ExceptResponse<T>
    {
        public ExceptResponse<T> And(Expression<Func<T, object>> property)
        {
            OverrideTrackingResponse<T>.SkipProperty(property);
            return new ExceptResponse<T>();
        }
    }
}