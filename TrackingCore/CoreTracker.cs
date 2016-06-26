using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using TrackingCore.Configuration;
using TrackingCore.Events;
using TrackingCore.Interfaces;
using TrackingCore.Models;

namespace TrackingCore
{
    internal class CoreTracker
    {
        public event EventHandler<DatabaseChangeEventArgs> OnDatabaseChange;

        private readonly ITrackerContext _context;

        public CoreTracker(ITrackerContext context)
        {
            _context = context;
        }

        public void AuditChanges(object userName, ExpandoObject metadata)
        {
            // Get all Deleted/Modified entities (not Unmodified or Detached or Added)
            foreach (
                DbEntityEntry ent in
                    _context.ChangeTracker.Entries()
                        .Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                EventType eventType = GetEventType(ent);

                DatabaseChangeEventArgsFactory databaseChangeEventArgsFactory = new DatabaseChangeEventArgsFactory(ent);
                DatabaseChangeEventArgs arg = databaseChangeEventArgsFactory
                    .CreateEventArgs(userName, eventType, _context, metadata, ent.Entity);

                if (arg != null)
                {
                    RaiseOnDatabaseChange(this, arg);
                }
            }
        }

        private EventType GetEventType(DbEntityEntry entry)
        {
            if (entry.State == EntityState.Deleted) return EventType.Deleted;

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

            return EventType.Modified;
        }

        public IEnumerable<DbEntityEntry> GetAdditions()
        {
            return _context.ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
        }

        public void AuditAdditions(object userName, IEnumerable<DbEntityEntry> addedEntries, ExpandoObject metadata)
        {
            foreach (DbEntityEntry ent in addedEntries)
            {
                var auditer = new DatabaseChangeEventArgsFactory(ent);

                DatabaseChangeEventArgs arg = auditer.CreateEventArgs(
                    userName, EventType.Added, _context, metadata, ent.Entity);
                if (arg != null)
                {
                    RaiseOnDatabaseChange(this, arg);
                }
            }
        }

        protected virtual void RaiseOnDatabaseChange(object sender, DatabaseChangeEventArgs e)
        {
            OnDatabaseChange?.Invoke(sender, e);
        }
    }
}
