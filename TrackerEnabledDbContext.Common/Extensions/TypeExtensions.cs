using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<string, bool> IsTrackingEnabledCache =
            new ConcurrentDictionary<string, bool>();

        private static readonly ConcurrentDictionary<string, IEnumerable<string>> PrimaryKeyNameCache =
            new ConcurrentDictionary<string, IEnumerable<string>>();

        private static readonly ConcurrentDictionary<string, string> TableNameCache =
            new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> ColumnNameCache =
            new ConcurrentDictionary<string, string>();

        #region Constants

        private const string ProxyNamespace = @"System.Data.Entity.DynamicProxies";

        #endregion

        #region -- public methods--

        public static Type GetEntityType(this Type entityType)
        {
            if (entityType.Namespace == ProxyNamespace)
            {
                return GetEntityType(entityType.BaseType);
            }

            return entityType;
        }

        public static IEnumerable<string> GetPrimaryKeyNames(this Type type, ITrackerContext context)
        {
            string key = type.FullName;
            return GetFromCache(PrimaryKeyNameCache, key, k => GetCachedPrimaryKeyNames(k, type, context));
        }

        public static bool IsTrackingEnabled(this Type type)
        {
            string key = string.Format("table__{0}", type.FullName);
            return GetFromCache(IsTrackingEnabledCache, key, k => IsTrackingEnabledForTable(k, type));
        }

        public static bool IsTrackingEnabled(this Type type, string propertyName)
        {
            string key = string.Format("column__{0}_{1}", type.FullName, propertyName);
            return GetFromCache(IsTrackingEnabledCache, key, k => IsTrackingEnabledForProperty(k, type, propertyName));
        }

        public static string GetTableName(this Type entityType, ITrackerContext context)
        {
            string key = entityType.FullName;
            return GetFromCache(TableNameCache, key, k => GetCachedTableName(entityType, context));
        }

        public static string GetColumnName(this Type type, string propertyName)
        {
            string key = string.Format("{0}_{1}", type.FullName, propertyName);
            return GetFromCache(ColumnNameCache, key, k => GetCachedColumnName(type, propertyName));
        }

        #endregion

        #region Private Methods

        private static IEnumerable<string> GetCachedPrimaryKeyNames(string key, Type type, ITrackerContext context)
        {
            ObjectContext objectContext = ((IObjectContextAdapter) context).ObjectContext;
            EdmType edmType;

            if (objectContext.MetadataWorkspace.TryGetType(type.Name, type.Namespace, DataSpace.OSpace, out edmType))
            {
                return edmType.MetadataProperties.Where(mp => mp.Name == "KeyMembers")
                    .SelectMany(mp => mp.Value as ReadOnlyMetadataCollection<EdmMember>)
                    .OfType<EdmProperty>().Select(edmProperty => edmProperty.Name);
            }
            throw new Exception(string.Format("could not find type '{0}' from objectContext", type.FullName));
        }

        private static bool IsTrackingEnabledForTable(string key, Type type)
        {
            Type entityType = type.GetEntityType();
            TrackChangesAttribute trackChangesAttribute =
                entityType.GetCustomAttributes(false).OfType<TrackChangesAttribute>().SingleOrDefault();
            return trackChangesAttribute != null && trackChangesAttribute.Enabled;
        }

        private static bool IsTrackingEnabledForProperty(string key, Type type, string propertyName)
        {
            Type entityType = type.GetEntityType();
            SkipTrackingAttribute skipTrackingAttribute =
                entityType.GetProperty(propertyName)
                    .GetCustomAttributes(false)
                    .OfType<SkipTrackingAttribute>()
                    .SingleOrDefault();
            return skipTrackingAttribute == null || !skipTrackingAttribute.Enabled;
        }

        private static string GetCachedTableName(Type entityType, ITrackerContext context)
        {
            var tableAttribute =
                entityType.GetCustomAttributes(typeof (TableAttribute), false).SingleOrDefault() as TableAttribute;
            string dbsetPropertyName =
                context.GetType()
                    .GetProperties()
                    .Single(x => x.PropertyType.GenericTypeArguments.Any(y => y == entityType))
                    .Name;

            // Get table name (if it has a Table attribute, use that, otherwise dbset property name)
            string tableName = (tableAttribute != null) ? tableAttribute.Name : dbsetPropertyName;
            return tableName;
        }

        private static string GetCachedColumnName(this Type type, string propertyName)
        {
            string columnName = propertyName;
            Type entityType = type.GetEntityType();
            var columnAttribute = entityType.GetProperty(propertyName).GetCustomAttribute<ColumnAttribute>(false);
            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
            {
                columnName = columnAttribute.Name;
            }
            return columnName;
        }

        #endregion

        private static TVal GetFromCache<TKey, TVal>(ConcurrentDictionary<TKey, TVal> dictionary, TKey key,
            Func<TKey, TVal> valueFactory)
        {
            lock (dictionary)
            {
                return dictionary.GetOrAdd(key, valueFactory);
            }
        }
    }
}