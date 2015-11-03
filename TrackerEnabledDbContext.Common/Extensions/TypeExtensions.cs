using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class TypeExtensions
    {
        #region Constants

        //todo:improve this recognition
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

        public static KeyValuePair<string, string> GetKeyValuePair<TEntity>(this TEntity entity, Expression<Func<TEntity, object>> property)
        {
            return new KeyValuePair<string, string>(property.GetPropertyInfo().Name, GetPropertyValue(property, entity)?.ToString());
        }

        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Underlying Type</typeparam>
        /// <param name="type">Nullable Type</param>
        /// <returns></returns>
        public static bool IsNullable<T>(this Type type)
        {
            return Nullable.GetUnderlyingType(type) == typeof(T);
        }

        public static object GetPropertyValue(this object entity, string propertyName)
        {
            return entity.GetType().GetPropertyValue(propertyName);
        }

        private static TValue GetPropertyValue<TEntity, TValue>(Expression<Func<TEntity, TValue>> property, TEntity entity)
        {
            return property.Compile()(entity);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = GetMember(propertyLambda);
            
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException("Expression is not a valid property.");

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expresion refers to a property that is not from type {type.Name}.");

            return propInfo;
        }

        private static MemberExpression GetMember<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is MemberExpression)
            {
                return (MemberExpression) propertyLambda.Body;
            }

            if (propertyLambda.Body is UnaryExpression)
            {
                return (MemberExpression)(((UnaryExpression)propertyLambda.Body).Operand);
            }

            throw new ArgumentException( $"Expression '{propertyLambda.Name}' refers is not a member expression or unary expression.");
        }
        #endregion
    }
}