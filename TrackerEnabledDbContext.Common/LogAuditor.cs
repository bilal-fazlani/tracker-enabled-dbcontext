using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public class LogAuditor : IDisposable
    {
        private readonly DbEntityEntry _dbEntry;

        public LogAuditor(DbEntityEntry dbEntry)
        {
            _dbEntry = dbEntry;
        }

        public void Dispose()
        {
        }

        public AuditLog CreateLogRecord(object userName, EventType eventType, ITrackerContext context)
        {
            Type entityType = _dbEntry.Entity.GetType().GetEntityType();
            if (typeof (IAuditSkippable).IsAssignableFrom(entityType)) return null;

            DateTime changeTime = DateTime.UtcNow;

            if (!EntityTrackingConfiguration.IsTrackingEnabled(entityType))
            {
                return null;
            }

            //todo: make this a static class
            var mapping = new DbMapping(context, entityType);

            List<PropertyConfiguerationKey> keyNames = mapping.PrimaryKeys().ToList();

            var newlog = new AuditLog
            {
                UserName = userName?.ToString(),
                EventDateUTC = changeTime,
                EventType = eventType,
                TypeFullName = entityType.FullName,
                RecordId = _dbEntry.GetPrimaryKeyValues(keyNames).ToString()
            };

            using (var detailsAuditor = (eventType == EventType.Added)
                ? new AddedLogDetailsAuditor(_dbEntry, newlog)
                : new LogDetailsAuditor(_dbEntry, newlog))
            {
                newlog.LogDetails = detailsAuditor.CreateLogDetails().ToList();
            }

            return newlog;
        }
    }
}