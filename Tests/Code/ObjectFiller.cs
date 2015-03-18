using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class ObjectFiller<ObjectType> where ObjectType : class
    {
        static Predicate<string> _propertyNameIgnoreRule = null;

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
