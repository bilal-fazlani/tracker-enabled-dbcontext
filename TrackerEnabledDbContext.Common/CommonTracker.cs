using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public static class CommonTracker
    {
        public static void AuditChanges(ITrackerContext dbContext, object userName)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (var ent in dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                using (var auditer = new LogAuditor(ent))
                {
                    var record = auditer.CreateLogRecord(userName,
                    ent.State == EntityState.Modified ? EventType.Modified : EventType.Deleted, dbContext);
                    if (record != null)
                    {
                        dbContext.AuditLog.Add(record);
                    }
                }
            }
        }

        public static IEnumerable<DbEntityEntry> GetAdditions(ITrackerContext dbContext)
        {
            return dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
        }

        public static void AuditAdditions(ITrackerContext dbContext, object userName, IEnumerable<DbEntityEntry> addedEntries)
        {
            // Get all Added entities
            foreach (var ent in addedEntries)
            {
                using (var auditer = new LogAuditor(ent))
                {
                    var record = auditer.CreateLogRecord(userName, EventType.Added, dbContext);
                    if (record != null)
                    {
                        dbContext.AuditLog.Add(record);
                    }
                }
            }
        }

        /// <summary>
        /// Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <returns></returns>
        public static IEnumerable<AuditLog> GetLogs<TTable>(ITrackerContext context)
        {
            var tableName = typeof(TTable).GetTableName(context);
            return context.AuditLog.Where(x => x.TableName == tableName);
        }

        /// <summary>
        /// Get all logs for the given table name
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns></returns>
        public static IEnumerable<AuditLog> GetLogs(ITrackerContext context, string tableName)
        {
            return context.AuditLog.Where(x => x.TableName == tableName);
        }

        /// <summary>
        /// Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public static IEnumerable<AuditLog> GetLogs<TTable>(ITrackerContext context, object primaryKey)
        {
            string key = primaryKey.ToString();
            var tableName = typeof(TTable).GetTableName(context);
            return context.AuditLog.Where(x => x.TableName == tableName && x.RecordId == key);
        }

        /// <summary>
        /// Get all logs for the given table name for a specific record
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public static IEnumerable<AuditLog> GetLogs(ITrackerContext context, string tableName, object primaryKey)
        {
            string key = primaryKey.ToString();
            return context.AuditLog.Where(x => x.TableName == tableName && x.RecordId == key);
        }

    }
}
