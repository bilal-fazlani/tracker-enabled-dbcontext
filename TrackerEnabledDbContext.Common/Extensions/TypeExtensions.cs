using System;
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

        public static PropertyInfo GetPropertyInfo<TSource>(this Expression<Func<TSource, object>> propertyLambda)
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

        #endregion
    }
}