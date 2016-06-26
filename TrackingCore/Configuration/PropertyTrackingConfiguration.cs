using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TrackingCore.Configuration
{
    internal static class PropertyTrackingConfiguration
    {
        internal static bool IsTrackingEnabled(PropertyConfiguerationKey property, Type entityType)
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

            bool trackValue = skipTrackingAttribute == null;

            return new TrackingConfigurationValue(trackValue);
        }
    }

}
