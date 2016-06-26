using System.Data.Entity.Infrastructure;
using TrackingCore.Models;

namespace TrackingCore.Auditors
{
    public class UnDeletedLogDetailsAudotor : ChangeLogDetailsAuditor
    {
        public UnDeletedLogDetailsAudotor(DbEntityEntry dbEntry) : base(dbEntry)
        {
        }
    }
}
