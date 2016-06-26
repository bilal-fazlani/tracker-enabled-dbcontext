using System.Collections.Generic;
using TrackingCore.Models;

namespace TrackingCore.Interfaces
{
    public interface ILogDetailsAuditor
    {
        IEnumerable<AuditLogDetail> CreateLogDetails();
    }
}