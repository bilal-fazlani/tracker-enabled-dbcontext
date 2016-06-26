using System.Data.Entity.Infrastructure;
using TrackingCore.Models;

namespace TrackingCore.Auditors
{
    public class SoftDeletedLogDetailsAuditor : ChangeLogDetailsAuditor
    {
        public SoftDeletedLogDetailsAuditor(DbEntityEntry dbEntry) : base(dbEntry)
        {
        }
    }
}
