using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TrackerEnabledDbContext.Common.Auditors;
using TrackerEnabledDbContext.Common.Configuration;
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
            foreach (
                DbEntityEntry ent in
                    dbContext.ChangeTracker.Entries()
                        .Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                using (var auditer = new LogAuditor(ent))
                {
                    var eventType = GetEventType(ent);

                    AuditLog record = auditer.CreateLogRecord(userName, eventType, dbContext);
                    if (record != null)
                    {
                        dbContext.AuditLog.Add(record);
                    }
                }
            }
        }

        private static EventType GetEventType(DbEntityEntry entry)
        {
            var isSoftDeletable = GlobalTrackingConfig.SoftDeletableType?.IsInstanceOfType(entry.Entity);

            if (isSoftDeletable != null && isSoftDeletable.Value)
            {
                var previouslyDeleted = (bool)entry.OriginalValues[GlobalTrackingConfig.SoftDeletablePropertyName];
                var nowDeleted = (bool)entry.CurrentValues[GlobalTrackingConfig.SoftDeletablePropertyName];

                if (previouslyDeleted && !nowDeleted)
                {
                    return EventType.UnDeleted;
                }

                if (!previouslyDeleted && nowDeleted)
                {
                    return EventType.SoftDeleted;
                }
            }

            var eventType = entry.State == EntityState.Modified ? EventType.Modified : EventType.Deleted;
            return eventType;
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
            return typeof(TEntity).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(entityType) || t.FullName == entityType.FullName).Select(m => m.FullName);
        }

        /// <summary>
        ///     Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs<TEntity>(ITrackerContext context)
        {
            IEnumerable<string> entityTypeNames = EntityTypeNames<TEntity>();
            return context.AuditLog.Where(x => entityTypeNames.Contains(x.TypeFullName));
        }

        /// <summary>
        ///     Get all logs for the enitity type name
        /// </summary>
        /// <param name="entityTypeName">Name of entity type</param>
        /// <returns></returns>
        public static IQueryable<AuditLog> GetLogs(ITrackerContext context, string entityTypeName)
        {
            return context.AuditLog.Where(x => x.TypeFullName == entityTypeName);
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
    }
}
