using System.Data.Entity.Infrastructure;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.EF6.Common.Auditors
{
    public class UnDeletedLogDetailsAudotor : ChangeLogDetailsAuditor
    {
        public UnDeletedLogDetailsAudotor(DbEntityEntry dbEntry, AuditLog log) : base(dbEntry, log)
        {
        }
    }
}
