using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public class PropertyTrackingConfiguration<T>
    {
        private EntityTrackingConfiguration<T> _entityConfiguration;

        internal PropertyTrackingConfiguration(EntityTrackingConfiguration<T> entityConfiguration)
        {
            _entityConfiguration = entityConfiguration;
        }

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
            var propertyInfo = property.GetPropertyInfo();
            return PropertyTrackingConfiguration.IsTrackingEnabled(
                new PropertyConfiguerationKey(propertyInfo.Name, propertyInfo.DeclaringType.FullName), typeof (T));
        }

        public EntityTrackingConfiguration<T> EntityConfiguration => _entityConfiguration;
    }

    internal static class PropertyTrackingConfiguration
    {
        internal static bool IsTrackingEnabled(PropertyConfiguerationKey property, Type entityType)
        {
            if (typeof(IUnTrackable).IsAssignableFrom(entityType)) return false;

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

            bool trackValue = skipTrackingAttribute == null;

            return new TrackingConfigurationValue(trackValue);
        }
    }

}
