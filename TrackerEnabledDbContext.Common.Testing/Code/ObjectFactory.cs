using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing
{
    public static class ObjectFactory<ObjectType> where ObjectType :class
    {
        static ObjectFactory()
        {
            ObjectFiller<ObjectType>.IgnorePropertiesWhen(propName => propName == "Id");
        }

        public static ObjectType Create(bool fill = true)
        {
            var instance = Activator.CreateInstance<ObjectType>();

            if (fill)
            {
                ObjectFiller<ObjectType>.Fill(instance);
            }

            return instance;
        }
    }
}
