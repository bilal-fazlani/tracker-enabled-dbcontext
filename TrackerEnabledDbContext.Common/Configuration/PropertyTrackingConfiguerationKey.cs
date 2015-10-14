using System;

namespace TrackerEnabledDbContext.Common.Configuration
{
    internal class PropertyConfiguerationKey
    {
        internal PropertyConfiguerationKey(string propertyName, string typeFullName)
        {
            PropertyName = propertyName;
            TypeFullName = typeFullName;
        }

        internal string PropertyName { get; }
        internal string TypeFullName { get; }

        public override bool Equals(object obj)
        {
            var otherEntity = (PropertyConfiguerationKey) obj;
            bool isNameSame = otherEntity.PropertyName.Equals(PropertyName, StringComparison.OrdinalIgnoreCase);
            bool isTypeSame = otherEntity.TypeFullName.Equals(TypeFullName, StringComparison.OrdinalIgnoreCase);

            return isNameSame && isTypeSame;
        }

        public override int GetHashCode()
        {
            return (PropertyName + TypeFullName).GetHashCode();
        }

        public override string ToString()
        {
            return TypeFullName + "." + PropertyName;
        }
    }
}