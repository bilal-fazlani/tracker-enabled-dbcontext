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
            return new KeyValuePair<string, string>(property.GetPropertyInfo().Name, GetPropertyValue(property, entity).ToString());
        }

        private static TValue GetPropertyValue<TEntity, TValue>(Expression<Func<TEntity, TValue>> property, TEntity entity)
        {
            return property.Compile()(entity);
        }

        public static PropertyInfo GetPropertyInfo<TSource>(this Expression<Func<TSource, object>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = GetMember(propertyLambda);
            
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");

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