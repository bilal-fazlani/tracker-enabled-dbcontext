using System.Collections.Generic;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Interfaces
{
    public interface ILogDetailsAuditor
    {
        IEnumerable<AuditLogDetail> CreateLogDetails();
    }
}