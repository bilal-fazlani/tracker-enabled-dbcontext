using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
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

        public AuditLog CreateLogRecord(object userName, EventType eventType, ITrackerContext context)
        {
            var entityType = _dbEntry.Entity.GetType().GetEntityType();
            var changeTime = DateTime.UtcNow;

            if (!entityType.IsTrackingEnabled())
            {
                return null;
            }

            //Get primary key value (If you have more than one key column, this will need to be adjusted)
            var keyName = entityType.GetPrimaryKeyName();

            var newlog = new AuditLog
            {
                UserName = userName != null ? userName.ToString() : null,
                EventDateUTC = changeTime,
                EventType = eventType,
                TableName = entityType.GetTableName(context),
                RecordId = _dbEntry.GetDatabaseValue(keyName).ToString()
            };
            
            using (var detailsAuditor = new LogDetailsAuditor(_dbEntry, newlog))
            {
                newlog.LogDetails = detailsAuditor.CreateLogDetails().ToList();
            }

            return newlog;
        }

        public void Dispose()
        {
            
        }
    }
}
