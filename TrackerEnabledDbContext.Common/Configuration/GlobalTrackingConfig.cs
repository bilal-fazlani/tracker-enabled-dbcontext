using System;
using System.Linq.Expressions;
using TrackerEnabledDbContext.Common.Extensions;
using System.Security.Policy;

namespace TrackerEnabledDbContext.Common.Configuration
{
    public static class GlobalTrackingConfig
    {
        //todo:unit test global config
        public static bool Enabled { get; set; } = true;

        /// <summary>
        /// This will clear all the configuration done by tracking fluent API.
        /// There is usually no purpose of this method, except for unit testing.
        /// </summary>
        public static void ClearFluentConfiguration()
        {
            TrackingDataStore.EntityConfigStore.Clear();
            TrackingDataStore.PropertyConfigStore.Clear();
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
