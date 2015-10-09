using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
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
