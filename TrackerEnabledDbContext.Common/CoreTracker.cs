﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using TrackerEnabledDbContext.Common.Auditors;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.EventArgs;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    public class CoreTracker
    {
        private ILogger _logger = null;

        public event EventHandler<AuditLogGeneratedEventArgs> OnAuditLogGenerated;

        private readonly ITrackerContext _context;

        public CoreTracker(ITrackerContext context)
        {
            _context = context;
        }

        public void AuditChanges(object userName)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (
                DbEntityEntry ent in
                    _context.ChangeTracker.Entries()
                        .Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                using (var auditer = new LogAuditor(ent))
                {
                    var eventType = GetEventType(ent);

                    AuditLog record = auditer.CreateLogRecord(userName, eventType, _context);

                    if (record != null)
                    {
                        var arg = new AuditLogGeneratedEventArgs(record, ent.Entity);
                        RaiseOnAuditLogGenerated(this, arg);

                        if (!arg.SkipSaving)
                        {
                            _context.AuditLog.Add(record);
                        }

                        if (!arg.SkipSavingLogToSerilog)
                        {
                            LogToLogger(arg.Log);
                        }
                    }
                }
            }
        }

        private EventType GetEventType(DbEntityEntry entry)
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

        public IEnumerable<DbEntityEntry> GetAdditions()
        {
            return _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
        }

        public void AuditAdditions(object userName, IEnumerable<DbEntityEntry> addedEntries)
        {
            // Get all Added entities
            foreach (DbEntityEntry ent in addedEntries)
            {
                using (var auditer = new LogAuditor(ent))
                {
                    AuditLog record = auditer.CreateLogRecord(userName, EventType.Added, _context);
                    if (record != null)
                    {
                        var arg = new AuditLogGeneratedEventArgs(record, ent.Entity);
                        RaiseOnAuditLogGenerated(this, arg);

                        if (!arg.SkipSaving)
                        {
                            _context.AuditLog.Add(record);
                        }

                        if (!arg.SkipSavingLogToSerilog)
                        {
                            LogToLogger(arg.Log);
                        }
                    }
                }
            }
        }

        private IEnumerable<string> EntityTypeNames<TEntity>()
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
        public IQueryable<AuditLog> GetLogs<TEntity>()
        {
            IEnumerable<string> entityTypeNames = EntityTypeNames<TEntity>();
            return _context.AuditLog.Where(x => entityTypeNames.Contains(x.TypeFullName));
        }

        /// <summary>
        ///     Get all logs for the enitity type name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityTypeName">Name of entity type</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string entityTypeName)
        {
            return _context.AuditLog.Where(x => x.TypeFullName == entityTypeName);
        }

        /// <summary>
        ///     Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <param name="context"></param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs<TEntity>(object primaryKey)
        {
            string key = primaryKey.ToString();
            IEnumerable<string> entityTypeNames = EntityTypeNames<TEntity>();

            return _context.AuditLog.Where(x => entityTypeNames.Contains(x.TypeFullName) && x.RecordId == key);
        }

        /// <summary>
        ///     Get all logs for the given entity name for a specific record
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityTypeName">entity type name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string entityTypeName, object primaryKey)
        {
            string key = primaryKey.ToString();
            return _context.AuditLog.Where(x => x.TypeFullName == entityTypeName && x.RecordId == key);
        }

        protected virtual void RaiseOnAuditLogGenerated(object sender, AuditLogGeneratedEventArgs e)
        {
            OnAuditLogGenerated?.Invoke(sender, e);
        }

        public void AddLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void AddLogger(ILogger logger,
            string messageTemplate, 
            Func<LogInfo, object[]> parameters)
        {
            _logger = logger;
            _messageTemplate = messageTemplate;
            _parameters = parameters;
        }

        private string _messageTemplate = "{@log}";
        private Func<LogInfo, object[]> _parameters;

        private void LogToLogger(AuditLog auditLog)
        {
            LogInfo log = new LogInfo(auditLog);

            object[] paramters = _parameters?.Invoke(log) ?? new object[] {log};

            _logger?.Information(_messageTemplate, paramters);
        }
    }
}
