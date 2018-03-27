using System;
using System.Linq.Expressions;
using TrackerEnabledDbContext.Common.Extensions;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class GlobalTrackingConfig
    {
        public static bool Enabled { get; set; } = true;

        public static bool TrackEmptyPropertiesOnAdditionAndDeletion { get; set; } = false;

        public static bool DisconnectedContext { get; set; } = false;

        /// <summary>
        /// This will clear all the configuration done by tracking fluent API.
        /// There is usually no purpose of this method, except for unit testing.
        /// </summary>
        public static void ClearFluentConfiguration()
        {
            TrackingDataStore.EntityConfigStore.Clear();
            TrackingDataStore.PropertyConfigStore.Clear();

            SoftDeletableType = null;
            SoftDeletablePropertyName = null;
        }

        internal static Type SoftDeletableType;
        internal static string SoftDeletablePropertyName;

        public static void SetSoftDeletableCriteria<TSoftDeletable>(
            Expression<Func<TSoftDeletable,bool>> softDeletableProperty)
        {
            SoftDeletableType = typeof (TSoftDeletable);
            SoftDeletablePropertyName = softDeletableProperty.GetPropertyInfo().Name;
        }
    }
}
