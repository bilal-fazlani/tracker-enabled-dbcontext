using System;
using System.ComponentModel;
using System.Reflection;

namespace TrackerEnabledDbContext.Common.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static void SetValueFromString(this PropertyInfo propertyInfo, object entity, string value)
        {
            Type propertyType = propertyInfo.PropertyType;
            TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
            object convertedValue = converter.ConvertFromString(value);
            propertyInfo.SetValue(entity, convertedValue);
        }
    }
}