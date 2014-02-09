using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext
{
    public static class Helper
    {
        private const string ProxyNamespace = @"System.Data.Entity.DynamicProxies";

        public static Type GetEntityType(Type entityType)
        {
            if (entityType.Namespace == ProxyNamespace)
            {
                return GetEntityType(entityType.BaseType);
            }
            else
            {
                return entityType;
            }
        }
    }
}
