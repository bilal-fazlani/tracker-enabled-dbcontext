using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.EventArgs
{
    public class AuditLogGeneratedEventArgs : System.EventArgs
    {
        public AuditLogGeneratedEventArgs(AuditLog log, object entity)
        {
            Log = log;
            Entity = entity;
        }

        public AuditLog Log { get; internal set; }

        public object Entity { get; internal set; }

        public bool SkipSaving { get; set; } = false;

        public bool SkipSavingToSerilog { get; set; } = false;
    }
}
