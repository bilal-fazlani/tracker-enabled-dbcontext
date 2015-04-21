using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common
{
    /// <summary>
    /// Creates AuditLogDetails for entries added in a previous call to SaveChanges.
    /// </summary>
    public class AddedLogDetailsAuditor : LogDetailsAuditor
    {
        public AddedLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }

        /// <summary>
        /// Treat unchanged entries as added entries when creating audit records.
        /// </summary>
        /// <param name="dbEntry"></param>
        /// <returns></returns>
        protected override EntityState StateOf(DbEntityEntry dbEntry)
        {
            if (dbEntry.State == EntityState.Unchanged)
            {
                return EntityState.Added;
            }

            return base.StateOf(dbEntry);
        }
    }
}
