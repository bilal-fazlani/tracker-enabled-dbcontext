using System.Collections.Generic;
using System.Data.Entity;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Interfaces
{
    public interface ITrackerContext : IDbContext
    {
        DbSet<AuditLog> AuditLog { get; set; }
        DbSet<AuditLogDetail> LogDetails { get; set; }

        IEnumerable<AuditLog> GetLogs(string tableName);
        IEnumerable<AuditLog> GetLogs(string tableName, object primaryKey);
        IEnumerable<AuditLog> GetLogs<TTable>();
        IEnumerable<AuditLog> GetLogs<TTable>(object primaryKey);
        int SaveChanges(object userName);
    }
}