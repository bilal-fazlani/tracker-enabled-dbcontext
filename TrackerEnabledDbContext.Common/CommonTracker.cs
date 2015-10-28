using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TrackerEnabledDbContext.Common.Auditors;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    using System.Linq.Expressions;

    public static class CommonTracker
    {
        public static void AuditChanges(ITrackerContext dbContext, object userName)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (
                DbEntityEntry ent in
                    dbContext.ChangeTracker.Entries()
                        .Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                using (var auditer = new LogAuditor(ent))
                {
                    AuditLog record = auditer.CreateLogRecord(userName,
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

        public static void AuditAdditions(ITrackerContext dbContext, object userName,
            IEnumerable<DbEntityEntry> addedEntries)
        {
            // Get all Added entities
            foreach (DbEntityEntry ent in addedEntries)
            {
                using (var auditer = new LogAuditor(ent))
                {
                    AuditLog record = auditer.CreateLogRecord(userName, EventType.Added, dbContext);
                    if (record != null)
                    {
                        dbContext.AuditLog.Add(record);
                    }
                }
            }
        }

        private static IEnumerable<string> EntityTypeNames<TEntity>()
        {
            Type entityType = typeof(TEntity);
            return typeof(TEntity).Assembly.GetTypes().Where(t => t.IsSubclassOf(entityType) || t.FullName == entityType.FullName).Select(m => m.FullName);
        }

        /// <summary>
        ///     Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs<TEntity>(ITrackerContext context)
        {
            IEnumerable<string> entityTypeNames = EntityTypeNames<TEntity>();
            string entityTypeName = typeof(TEntity).Name;
            return context.AuditLog.Where(x => entityTypeNames.Contains(x.TypeFullName));
        }

        /// <summary>
        ///     Get all logs for the enitity type name
        /// </summary>
        /// <param name="entityTypeName">Name of entity type</param>
        /// <param name="includeExpressions">Expressions designating the sub entities that are to be Included in the return. The main usage for this
        /// would be to include LogDetails. Here is a sample call <code>GetLogs(this, "MyEntity", entity=> entity.LogDetails)</code>
        /// </param>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs(ITrackerContext context, 
            string entityTypeName, 
            params Expression<Func<AuditLog, object>>[] includeExpressions)
        {
            var query = context.AuditLog.AsQueryable();

            if (includeExpressions != null && includeExpressions.Length > 0)
            {
                query = includeExpressions.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.Where(x => x.TypeFullName == entityTypeName);

        }
        
        /// <summary>
        ///     Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs<TEntity>(ITrackerContext context, object primaryKey)
        {
            string key = primaryKey.ToString();
            string entityTypeName = typeof(TEntity).Name;
            IEnumerable<string> entityTypeNames = EntityTypeNames<TEntity>();

            return context.AuditLog.Where(x => entityTypeNames.Contains(x.TypeFullName) && x.RecordId == key);
        }

        /// <summary>
        ///     Get all logs for the given entity name for a specific record
        /// </summary>
        /// <param name="entityTypeName">entity type name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs(ITrackerContext context, string entityTypeName, object primaryKey)
        {
            string key = primaryKey.ToString();
            return context.AuditLog.Where(x => x.TypeFullName == entityTypeName && x.RecordId == key);
        }

        /// <summary>
        ///     Get the id of the most recently created log for the given table name for a specific record
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns>Log id</returns>
        public static long GetLastAuditLogId(ITrackerContext context, string tableName, object primaryKey)
        {
            string key = primaryKey.ToString();
            return context.AuditLog.Where(x => x.TypeFullName == tableName && x.RecordId == key).OrderByDescending(x => x.AuditLogId).Select(x => x.AuditLogId).FirstOrDefault();
        }
    }
}
