using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using TrackingCore.Auditors;
using TrackingCore.Configuration;
using TrackingCore.Extensions;
using TrackingCore.Interfaces;
using TrackingCore.Models;

namespace TrackingCore.Events
{
    internal class AuditLogGeneratedEventArgsFactory
    {
        private readonly DbEntityEntry _dbEntry;

        public AuditLogGeneratedEventArgsFactory(DbEntityEntry dbEntry)
        {
            _dbEntry = dbEntry;
        }

        internal AuditLogGeneratedEventArgs CreateEventArgs(
            object userName, EventType eventType, 
            ITrackerContext context, 
            ExpandoObject metadata,
            object entity)
        {
            DateTime changeTime = DateTime.Now;

            Type entityType = _dbEntry.Entity.GetType().GetEntityType();
            if (!EntityTrackingConfiguration.IsTrackingEnabled(entityType)) return null;

            var detailsAuditor = LogDetailsAuditorFactory.GetDetailsAuditor(eventType, _dbEntry);
            var logDetails = detailsAuditor.CreateLogDetails().ToList();
            if (!logDetails.Any()) return null;

            DbMapping mapping = new DbMapping(context, entityType);
            List<PropertyConfiguerationKey> keyNames = mapping.PrimaryKeys().ToList();

            AuditLogGeneratedEventArgs args = new AuditLogGeneratedEventArgs
            {
                Metadata = metadata,
                Date = changeTime,
                EventType = eventType,
                Username = userName,
                EntityId = GetPrimaryKeyValuesOf(_dbEntry, keyNames).ToString(),
                Entity = entity,
                LogDetails = logDetails
            };

            return args;
        }
        

        //TODO: make this better
        private static object GetPrimaryKeyValuesOf(
            DbEntityEntry dbEntry,
            List<PropertyConfiguerationKey> properties)
        {
            if (properties.Count == 1)
            {
                return dbEntry.GetDatabaseValues().GetValue<object>(properties.Select(x => x.PropertyName).First());
            }
            if (properties.Count > 1)
            {
                string output = "[";

                output += string.Join(",",
                    properties.Select(colName => dbEntry.GetDatabaseValues().GetValue<object>(colName.PropertyName)));

                output += "]";
                return output;
            }
            throw new KeyNotFoundException("key not found for " + dbEntry.Entity.GetType().FullName);
        }
    }
}