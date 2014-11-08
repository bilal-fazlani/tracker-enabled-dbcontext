using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public static class CommonTracker
    {
        public static int SaveChanges(ITrackerContext dbContext, object userName)
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

            var addedEntries = dbContext.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            var result = dbContext.SaveChanges();
            //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

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

            //save changed to audit of added entries
            dbContext.SaveChanges();
            return result;
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
