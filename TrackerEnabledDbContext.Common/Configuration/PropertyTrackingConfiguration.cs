using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrackerEnabledDbContext.Common.Extensions;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public class PropertyTrackingConfiguration<T>
    {
        public PropertyTrackingConfiguration<T> SkipProperty(Expression<Func<T, object>> property)
        {
            PropertyInfo info = property.GetPropertyInfo();
            var newValue = new TrackingConfigurationValue(false, TrackingConfigurationPriority.High);

            TrackingDataStore.PropertyConfigStore.AddOrUpdate(
                new PropertyConfiguerationKey(info.Name, info.DeclaringType.FullName),
                newValue,
                (existingKey, existingvalue) => newValue);

            return this;
        }

        public PropertyTrackingConfiguration<T> TrackProperty(Expression<Func<T, object>> property)
        {
            PropertyInfo info = property.GetPropertyInfo();
            var newValue = new TrackingConfigurationValue(true, TrackingConfigurationPriority.High);
            TrackingDataStore.PropertyConfigStore.AddOrUpdate(
                new PropertyConfiguerationKey(info.Name, info.DeclaringType.FullName),
                newValue, 
                (key, value) => newValue);

            return this;
        }

        public static bool IsTrackingEnabled(Expression<Func<T, object>> property)
        {
            var key = new PropertyConfiguerationKey(property.GetPropertyInfo().Name,
                property.GetPropertyInfo().DeclaringType.FullName);

            string propertyName = property.GetPropertyInfo().Name;

            var result = TrackingDataStore.PropertyConfigStore
                .GetOrAdd(key, 
                (x) =>
                PropertyTrackingConfiguration.PropertyConfigValueFactory(propertyName, typeof(T)));

            return result.Value;
        }
    }

    public static class PropertyTrackingConfiguration
    {
        public static bool IsTrackingEnabled(PropertyConfiguerationKey property, Type entityType)
        {
            var result = TrackingDataStore.PropertyConfigStore
                .GetOrAdd(property,
                (x) =>
                PropertyConfigValueFactory(property.PropertyName, entityType));

            return result.Value;
        }

        internal static TrackingConfigurationValue PropertyConfigValueFactory(string propertyName, 
            Type entityType)
        {
            SkipTrackingAttribute skipTrackingAttribute =
                entityType.GetProperty(propertyName)
                    .GetCustomAttributes(false)
                    .OfType<SkipTrackingAttribute>()
                    .SingleOrDefault();

            bool trackValue = skipTrackingAttribute == null || !skipTrackingAttribute.Enabled;

            return new TrackingConfigurationValue(trackValue);
        }
    }

}
