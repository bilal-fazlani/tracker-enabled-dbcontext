using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<string, bool> TableTrackerCache =
            new ConcurrentDictionary<string, bool>();

        private static readonly ConcurrentDictionary<string, bool> ColumnTrackerCache =
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
            return GetFromCache(PrimaryKeyNameCache, key, k => PrimaryKeyNamesFactory(type, context));
        }

        public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            Type type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda,
                    type));

            return propInfo;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TEntity>(Expression<Func<TSource, TEntity>> propertyLambda)
        {
            Type type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda));

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda,
                    type));

            return propInfo;
        }

        public static bool IsTrackingEnabled(this Type type)
        {
            return GetFromCache(TableTrackerCache, type.FullName, k => TableTrackerIndicatorFactory(type));
        }

        public static bool IsTrackingEnabled(this Type type, string propertyName)
        {
            string key = GetFullPropertyName(type, propertyName);
            return GetFromCache(ColumnTrackerCache, key, k => ColumnTrackerIndicatorFactory(type, propertyName));
        }

        public static string GetTableName(this Type entityType, ITrackerContext context)
        {
            string key = entityType.FullName;
            return GetFromCache(TableNameCache, key, k => TableNameFactory(entityType, context));
        }

        public static string GetColumnName(this Type type, string propertyName)
        {
            string key = GetFullPropertyName(type, propertyName);
            return GetFromCache(ColumnNameCache, key, k => ColumnNameFactory(type, propertyName));
        }

        #endregion

        #region Private Methods

        private static string GetColumnTrackerCacheKey<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            var propInfo = GetPropertyInfo(propertyLambda);
            string propertyName = propInfo.Name;
            Type type = propInfo.DeclaringType;

            return GetFullPropertyName(type, propertyName);
        }

        private static string GetFullPropertyName(Type type, string propertyName)
        {
            return type.FullName + "." + propertyName;
        }

        private static IEnumerable<string> PrimaryKeyNamesFactory(Type type, ITrackerContext context)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            EdmType edmType;

            if (objectContext.MetadataWorkspace.TryGetType(type.Name, type.Namespace, DataSpace.OSpace, out edmType))
            {
                return edmType.MetadataProperties.Where(mp => mp.Name == "KeyMembers")
                    .SelectMany(mp => mp.Value as ReadOnlyMetadataCollection<EdmMember>)
                    .OfType<EdmProperty>().Select(edmProperty => edmProperty.Name);
            }
            throw new Exception(string.Format("could not find type '{0}' from objectContext", type.FullName));
        }

        private static bool TableTrackerIndicatorFactory(Type type)
        {
            Type entityType = type.GetEntityType();
            TrackChangesAttribute trackChangesAttribute =
                entityType.GetCustomAttributes(false).OfType<TrackChangesAttribute>().SingleOrDefault();
            return trackChangesAttribute != null && trackChangesAttribute.Enabled;
        }

        private static bool ColumnTrackerIndicatorFactory(Type type, string propertyName)
        {
            Type entityType = type.GetEntityType();
            SkipTrackingAttribute skipTrackingAttribute =
                entityType.GetProperty(propertyName)
                    .GetCustomAttributes(false)
                    .OfType<SkipTrackingAttribute>()
                    .SingleOrDefault();
            return skipTrackingAttribute == null || !skipTrackingAttribute.Enabled;
        }

        private static string TableNameFactory(Type entityType, ITrackerContext context)
        {
            var tableAttribute =
                entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
            string dbsetPropertyName =
                context.GetType()
                    .GetProperties()
                    .Single(x => x.PropertyType.GenericTypeArguments.Any(y => y == entityType || entityType.IsSubclassOf(y)))
                    .Name;

            // Get table name (if it has a Table attribute, use that, otherwise dbset property name)
            string tableName = (tableAttribute != null) ? tableAttribute.Name : dbsetPropertyName;
            return tableName;
        }

        private static string ColumnNameFactory(this Type type, string propertyName)
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

        private static IEnumerable<string> GetEntityColumnNames<TEntity>()
        {
            return GetEntityColumnNames(typeof (TEntity));
        }

        private static IEnumerable<string> GetEntityColumnNames(Type type)
        {
            Type entityType = type.GetEntityType();

            foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance ))
            {
                string columnName = prop.Name;
                var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>(false);
                if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                {
                    columnName = columnAttribute.Name;
                }
                yield return columnName;
            }
        }

        #endregion

        #region internal -
        internal static void EnableTableTracking<TEntity>()
        {
            TableTrackerCache.TryAdd(typeof(TEntity).FullName, true);

            var propertyNames = GetEntityColumnNames<TEntity>();
            foreach (var propertyName in propertyNames)
            {
                IsTrackingEnabled(typeof(TEntity), propertyName);
            }
        }

        internal static void SkipTrackingFor<TSource>(Expression<Func<TSource, object>> propertyLambda)
        {
            var key = GetColumnTrackerCacheKey(propertyLambda);
            bool removeSuccess;
            TableTrackerCache.TryRemove(key, out removeSuccess);
        }

        #endregion --

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