﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.EventArgs;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Identity
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
         Justification = "False positive.  IDisposable is inherited via DbContext.  See http://stackoverflow.com/questions/8925925/code-analysis-ca1063-fires-when-deriving-from-idisposable-and-providing-implemen for details.")]
    public class TrackerIdentityContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> :
        IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>, ITrackerContext
        
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim> 
        where TRole : IdentityRole<TKey, TUserRole> 
        where TUserLogin : IdentityUserLogin<TKey> 
        where TUserRole : IdentityUserRole<TKey> 
        where TUserClaim : IdentityUserClaim<TKey>
    {
        private readonly CoreTracker _coreTracker;

        private Func<string> _usernameFactory;
        private string _defaultUsername;
        private Action<dynamic> _metadataConfiguration;

        private bool _additionTrackingEnabled = true;
        private bool _modificationTrackingEnabled = true;
        private bool _deletionTrackingEnabled = true;

        public bool TrackingEnabled
        {
            get
            {
                return GlobalTrackingConfig.Enabled && (_additionTrackingEnabled ||
                                                               _modificationTrackingEnabled ||
                                                               _deletionTrackingEnabled);
            }
            set
            {
                AdditionTrackingEnabled = value;
                ModificationTrackingEnabled = value;
                DeletionTrackingEnabled = value;
            }
        }

        public bool AdditionTrackingEnabled
        {
            get
            {
                return GlobalTrackingConfig.AdditionsEnabled && _additionTrackingEnabled;
            }
            set
            {
                _additionTrackingEnabled = value;
            }
        }

        public bool ModificationTrackingEnabled
        {
            get
            {
                return GlobalTrackingConfig.ModificationsEnabled && _modificationTrackingEnabled;
            }
            set
            {
                _modificationTrackingEnabled = value;
            }
        }

        public bool DeletionTrackingEnabled
        {
            get
            {
                return GlobalTrackingConfig.DeletionsEnabled && _deletionTrackingEnabled;
            }
            set
            {
                _deletionTrackingEnabled = value;
            }
        }

        public virtual void ConfigureUsername(Func<string> usernameFactory)
        {
            _usernameFactory = usernameFactory;
        }

        public virtual void ConfigureUsername(string defaultUsername)
        {
            _defaultUsername = defaultUsername;
        }

        public virtual void ConfigureMetadata(Action<dynamic> metadataConfiguration)
        {
            _metadataConfiguration = metadataConfiguration;
        }

        public TrackerIdentityContext()
        {
            _coreTracker = new CoreTracker(this);
        }

        public TrackerIdentityContext(DbCompiledModel model) : base(model)
        {
            _coreTracker = new CoreTracker(this);
        }

        public TrackerIdentityContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            _coreTracker = new CoreTracker(this);
        }

        public TrackerIdentityContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            _coreTracker = new CoreTracker(this);
        }

        public TrackerIdentityContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            _coreTracker = new CoreTracker(this);
        }

        public TrackerIdentityContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            _coreTracker = new CoreTracker(this);
        }

        public virtual DbSet<AuditLog> AuditLog { get; set; }

        public virtual DbSet<AuditLogDetail> LogDetails { get; set; }

        public virtual event EventHandler<AuditLogGeneratedEventArgs> OnAuditLogGenerated
        {
            add { _coreTracker.OnAuditLogGenerated += value; }
            remove { _coreTracker.OnAuditLogGenerated -= value; }
        }

        /// <summary>
        ///     This method saves the model changes to the database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual int SaveChanges(object userName)
        {
            if (!TrackingEnabled)
            {
                return base.SaveChanges();
            }

            dynamic metadata = new ExpandoObject();
            _metadataConfiguration?.Invoke(metadata);

            if (ModificationTrackingEnabled) _coreTracker.AuditModifications(userName, metadata);
            if (DeletionTrackingEnabled) _coreTracker.AuditDeletions(userName, metadata);

            int result;
            if (AdditionTrackingEnabled)
            {
                IEnumerable<DbEntityEntry> addedEntries = _coreTracker.GetAdditions();
                // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
                result = base.SaveChanges();
                //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

                _coreTracker.AuditAdditions(userName, addedEntries, metadata);

                //save changes to audit of added entries
                base.SaveChanges();
            }
            else
            {
                //save changes
                result = base.SaveChanges();
            }

            return result;
        }

        /// <summary>
        ///     This method saves the model changes to the database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            if (!TrackingEnabled)
            {
                return base.SaveChanges();
            }

            return SaveChanges(_usernameFactory?.Invoke() ?? _defaultUsername);
        }

        /// <summary>
        ///     Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <returns></returns>
        public virtual IQueryable<AuditLog> GetLogs<TEntity>()
        {
            return _coreTracker.GetLogs<TEntity>();
        }

        /// <summary>
        ///     Get all logs for the given table name
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns></returns>
        public virtual IQueryable<AuditLog> GetLogs(string tableName)
        {
            return _coreTracker.GetLogs(tableName);
        }

        /// <summary>
        ///     Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TEntity">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public virtual IQueryable<AuditLog> GetLogs<TEntity>(object primaryKey)
        {
            return _coreTracker.GetLogs<TEntity>(primaryKey);
        }

        /// <summary>
        ///     Get all logs for the given table name for a specific record
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public virtual IQueryable<AuditLog> GetLogs(string tableName, object primaryKey)
        {
            return _coreTracker.GetLogs(tableName, primaryKey);
        }

        #region -- Async --

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <param name="cancellationToken">
        ///     A System.Threading.CancellationToken to observe while waiting for the task
        ///     to complete.
        /// </param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken)
        {
            if (!TrackingEnabled)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            dynamic metadata = new ExpandoObject();
            _metadataConfiguration?.Invoke(metadata);

            if (ModificationTrackingEnabled) _coreTracker.AuditModifications(userName, metadata);
            if (DeletionTrackingEnabled) _coreTracker.AuditDeletions(userName, metadata);

            int result;
            if (AdditionTrackingEnabled)
            {
                IEnumerable<DbEntityEntry> addedEntries = _coreTracker.GetAdditions();

                // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
                result = await base.SaveChangesAsync(cancellationToken);

                //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.
                _coreTracker.AuditAdditions(userName, addedEntries, metadata);

                //save changes to audit of added entries
                await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                //save changes
                result = await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChangesAsync() whenever possible.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(int userId)
        {
            if (!TrackingEnabled)
            {
                return await base.SaveChangesAsync(CancellationToken.None);
            }

            return await SaveChangesAsync(userId, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table.
        ///     Always use this method instead of SaveChangesAsync() whenever possible.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync(string userName)
        {
            if (!TrackingEnabled)
            {
                return await base.SaveChangesAsync(CancellationToken.None);
            }

            return await SaveChangesAsync(userName, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The task result
        ///     contains the number of objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync()
        {
            if (!TrackingEnabled)
            {
                return await base.SaveChangesAsync(CancellationToken.None);
            }

            return await SaveChangesAsync(_usernameFactory?.Invoke() ?? _defaultUsername, CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        ///     If the tracker for a table is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A System.Threading.CancellationToken to observe while waiting for the task
        ///     to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation.  The task result
        ///     contains the number of objects written to the underlying database.
        /// </returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (!TrackingEnabled)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }

            return await SaveChangesAsync(_usernameFactory?.Invoke() ?? _defaultUsername, cancellationToken);
        }

        #endregion --
    }

    public class TrackerIdentityContext<TUser> : 
        TrackerIdentityContext<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
        where TUser : IdentityUser
    {
        public TrackerIdentityContext()
        {
        }

        public TrackerIdentityContext(DbCompiledModel model) : base(model)
        {
        }

        public TrackerIdentityContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public TrackerIdentityContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        public TrackerIdentityContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public TrackerIdentityContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }
    }
}