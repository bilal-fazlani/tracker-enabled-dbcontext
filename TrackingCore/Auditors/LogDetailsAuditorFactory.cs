using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TrackingCore.Interfaces;
using TrackingCore.Models;

namespace TrackingCore.Auditors
{
    public class LogDetailsAuditorFactory
    {
        public static ILogDetailsAuditor GetDetailsAuditor(EventType eventType, DbEntityEntry dbEntry)
        {
            switch (eventType)
            {
                case EventType.Added:
                    return new AdditionLogDetailsAuditor(dbEntry);

                case EventType.Deleted:
                    return new DeletetionLogDetailsAuditor(dbEntry);

                case EventType.Modified:
                    return new ChangeLogDetailsAuditor(dbEntry);

                case EventType.SoftDeleted:
                    return new SoftDeletedLogDetailsAuditor(dbEntry);

                case EventType.UnDeleted:
                    return new UnDeletedLogDetailsAudotor(dbEntry);

                default:
                    return null;
            }
        }
    }
}