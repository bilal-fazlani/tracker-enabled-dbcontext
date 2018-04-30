using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
    internal static class PropertyTrackingConfiguration
    {
        internal static bool IsTrackingEnabled(PropertyConfigurationKey property, Type entityType)
        {
            if (typeof(IUnTrackable).IsAssignableFrom(entityType)) return false;

            var result = TrackingDataStore.PropertyConfigStore
                .GetOrAdd(property,
                (x) =>
                PropertyConfigValueFactory(property.PropertyName, entityType));

            return result.Value;
        }

        internal static TrackingConfigurationValue PropertyConfigValueFactory(string propertyName, Type entityType)
        {
            //if property is missing from the model assume it is not tracked
            PropertyInfo pi = entityType.GetProperty(propertyName);

            bool trackValue = false;
            if (pi != null)
            {
                SkipTrackingAttribute skipTrackingAttribute = pi.GetCustomAttributes(false)
                    .OfType<SkipTrackingAttribute>()
                    .SingleOrDefault();
                trackValue = skipTrackingAttribute == null;
            }

            return new TrackingConfigurationValue(trackValue);
        }
    }
}
