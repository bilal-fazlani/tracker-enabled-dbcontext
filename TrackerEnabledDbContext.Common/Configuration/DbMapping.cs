using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Configuration
{
    internal class DbMapping
    {
        private readonly IEntityMap _entityMap;
        private readonly Type _entityType;

        internal DbMapping(ITrackerContext context, Type entityType)
        {
            _entityType = entityType;
            _entityMap = (context as DbContext).Db(_entityType);
        }

        //public string GetTableName()
        //{
        //    return _entityMap.TableName;
        //}

        //public string GetColumnName(string propertyName)
        //{
        //    return _entityMap.Properties
        //        .Single(x => x.PropertyName == propertyName)
        //        .ColumnName;
        //}

        internal IEnumerable<PropertyConfiguerationKey> PrimaryKeys()
        {
            return _entityMap.Pks
                .Select(x => new PropertyConfiguerationKey(
                    x.PropertyName, 
                    _entityType.FullName));
        }
    }
}
