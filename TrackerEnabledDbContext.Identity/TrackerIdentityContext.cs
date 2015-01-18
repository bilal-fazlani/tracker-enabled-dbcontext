using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Identity
{
    public class TrackerIdentityContext<TUser> : IdentityDbContext<TUser>, ITrackerContext where TUser : IdentityUser
    {
        public TrackerIdentityContext(): base() { }

        public TrackerIdentityContext(string connectinString) : base(connectinString) { }

        // Summary:
        //     Constructor which takes the connection string to use
        //
        // Parameters:
        //   nameOrConnectionString:
        //
        //   throwIfV1Schema:
        //     Will throw an exception if the schema matches that of Identity 1.0.0
        public TrackerIdentityContext(string nameOrConnectionString, bool throwIfV1Schema) : base(nameOrConnectionString, throwIfV1Schema) { }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<AuditLogDetail> LogDetails { get; set; }

        /// <summary>
        /// This method saves the model changes to the database.
        /// If the tracker for of table is active, it will also put the old values in tracking table.
        /// Always use this method instead of SaveChanges() whenever possible.
        /// </summary>
        /// <param name="userName">Username of the logged in identity</param>
        /// <returns>Returns the number of objects written to the underlying database.</returns>
        public virtual int SaveChanges(object userName)
        {
            return CommonTracker.SaveChanges(this, userName);
        }

        /// <summary>
        /// Get all logs for the given model type
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetLogs<TTable>()
        {
            return CommonTracker.GetLogs<TTable>(this);
        }

        /// <summary>
        /// Get all logs for the given table name
        /// </summary>
        /// <param name="tableName">Name of table</param>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetLogs(string tableName)
        {
            return CommonTracker.GetLogs(this, tableName);
        }

        /// <summary>
        /// Get all logs for the given model type for a specific record
        /// </summary>
        /// <typeparam name="TTable">Type of domain model</typeparam>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetLogs<TTable>(object primaryKey)
        {
            return CommonTracker.GetLogs<TTable>(this, primaryKey);
        }

        /// <summary>
        /// Get all logs for the given table name for a specific record
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="primaryKey">primary key of record</param>
        /// <returns></returns>
        public IEnumerable<AuditLog> GetLogs(string tableName, object primaryKey)
        {
            return CommonTracker.GetLogs(this, tableName, primaryKey);
        }
    }
}
