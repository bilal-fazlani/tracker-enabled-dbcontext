using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Core.Common.Auditors
{
    public class SoftDeletedLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public SoftDeletedLogDetailsAuditor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }
    }
}
