using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Interfaces
{
    public interface ITrackerContext : IDbContext
    {
        DbSet<AuditLog> AuditLog { get; set; }
        DbSet<AuditLogDetail> LogDetails { get; set; }

        IQueryable<AuditLog> GetLogs(string tableName);
        IQueryable<AuditLog> GetLogs(string tableName, object primaryKey);
        IQueryable<AuditLog> GetLogs<TTable>();
        IQueryable<AuditLog> GetLogs<TTable>(object primaryKey);
        int SaveChanges(object userName);

        //async
        Task<int> SaveChangesAsync(object userName, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(int userId);
        Task<int> SaveChangesAsync(string userName);
    }
}