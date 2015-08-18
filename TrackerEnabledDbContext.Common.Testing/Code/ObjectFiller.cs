using System;

namespace TrackerEnabledDbContext.Common.Testing
{
    public static class ObjectFiller<ObjectType> where ObjectType : class
    {
        private static Predicate<string> _propertyNameIgnoreRule;

        public static void IgnorePropertiesWhen(Predicate<string> propertyNameIgnoreRule)
        {
            _propertyNameIgnoreRule = propertyNameIgnoreRule;
        }

        public static void Fill(ObjectType obj)
        {
            //TODO: implement code
            /*
            check _propertyNameIgnoreRule != null
            foreach property in obj.properties
            if(!_propertyNameIgnoreRule(property)
            {
                obj.property = randomValue();
            }
            */
        }
    }
}