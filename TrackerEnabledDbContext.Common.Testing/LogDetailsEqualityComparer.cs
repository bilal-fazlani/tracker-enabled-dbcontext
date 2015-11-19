using System.Collections.Generic;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Testing
{
    public class LogDetailsEqualityComparer : IEqualityComparer<AuditLogDetail>
    {
        public bool Equals(AuditLogDetail x, AuditLogDetail y)
        {
            return (x.PropertyName == y.PropertyName &&
                    x.OriginalValue == y.OriginalValue &&
                    x.NewValue == y.NewValue);
        }

        public int GetHashCode(AuditLogDetail obj)
        {
            return obj.NewValue.GetHashCode() + obj.OriginalValue.GetHashCode() + obj.PropertyName.GetHashCode();
        }
    }
}