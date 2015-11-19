using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Auditors
{
    public class SoftDeletedLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public SoftDeletedLogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }
    }
}
