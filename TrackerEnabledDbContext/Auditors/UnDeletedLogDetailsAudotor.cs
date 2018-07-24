using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Core.Common.Auditors
{
    public class UnDeletedLogDetailsAudotor : ChangeLogDetailsAuditor
    {
        public UnDeletedLogDetailsAudotor(EntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }
    }
}
