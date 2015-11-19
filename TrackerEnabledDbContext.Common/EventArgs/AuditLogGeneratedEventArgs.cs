using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.EventArgs
{
    public class AuditLogGeneratedEventArgs : System.EventArgs
    {
        public AuditLogGeneratedEventArgs(AuditLog log)
        {
            Log = log;
        }

        public AuditLog Log { get; internal set; }

        public bool SkipSaving { get; set; } = false;
    }
}
