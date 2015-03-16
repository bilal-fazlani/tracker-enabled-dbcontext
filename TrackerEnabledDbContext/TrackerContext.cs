using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext
{
    public class TrackerContext : DbContext, ITrackerContext
    {
        public TrackerContext()
            : base()
        {
            
        }

        public TrackerContext(string connectinString)
            : base(connectinString)
        {
            
        }

        public TrackerContext(DbConnection dbconnection, bool contextOwnsConnection)
            : base(dbconnection, contextOwnsConnection)
        {
            
        }
        
        public DbSet<AuditLog> AuditLog { get; set; }

        public DbSet<AuditLogDetail> LogDetails { get; set; }

        /// <summary>
        /// This method saves the model changes to the database.
        /// If the tracker for a table is active, it will also put the old values in tracking table.
        /// Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual int SaveChanges(object userName)
        {
            CommonTracker.AuditChanges(this, userName);

            var addedEntries = CommonTracker.GetAdditions(this);
            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            int result = base.SaveChanges();
            //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.

            CommonTracker.AuditAdditions(this, userName, addedEntries);

            //save changes to audit of added entries
            base.SaveChanges();
            return result;
        }

        /// <summary>
        /// This method saves the model changes to the database.
        /// If the tracker for a table is active, it will also put the old values in tracking table.
        /// </summary>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            return this.SaveChanges(null);
        }

        #region -- Async --
        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// If the tracker for a table is active, it will also put the old values in tracking table.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <param name="cancellationToken">
        /// A System.Threading.CancellationToken to observe while waiting for the task
        /// to complete.
        /// </param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public async virtual Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested == true)
                cancellationToken.ThrowIfCancellationRequested();

            CommonTracker.AuditChanges(this, userName);

            var addedEntries = CommonTracker.GetAdditions(this);

            // Call the original SaveChanges(), which will save both the changes made and the audit records...Note that added entry auditing is still remaining.
            int result = await base.SaveChangesAsync(cancellationToken);

            //By now., we have got the primary keys of added entries of added entiries because of the call to savechanges.
            CommonTracker.AuditAdditions(this, userName, addedEntries);

            //save changes to audit of added entries
            await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// If the tracker for a table is active, it will also put the old values in tracking table.
        /// Always use this method instead of SaveChangesAsync() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public async virtual Task<int> SaveChangesAsync(object userName)
        {
            return await this.SaveChangesAsync(userName, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// If the tracker for a table is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation.  The task result
        /// contains the number of objects written to the underlying database.
        /// </returns>
        public async override Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// If the tracker for a table is active, it will also put the old values in tracking table with null UserName.
        /// </summary>
        /// <param name="cancellationToken">
        /// A System.Threading.CancellationToken to observe while waiting for the task
        /// to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous save operation.  The task result
        /// contains the number of objects written to the underlying database.
        /// </returns>
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await this.SaveChangesAsync(cancellationToken);
        }
        #endregion

        /// <summary>
        /// Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs<TTable>()
        {
            return CommonTracker.GetLogs<TTable>(this);
        }

        /// <summary>
        /// Get all logs for the given table name
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string tableName)
        {
            return CommonTracker.GetLogs(this, tableName);
        }

        /// <summary>
        /// Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs<TTable>(object primaryKey)
        {
            return CommonTracker.GetLogs<TTable>(this, primaryKey);
        }

        /// <summary>
        /// Get all logs for the given table name for a specific record
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IQueryable<AuditLog> GetLogs(string tableName, object primaryKey)
        {
            return CommonTracker.GetLogs(this, tableName, primaryKey);
        }
    }
}
