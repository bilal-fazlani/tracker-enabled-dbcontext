using System;
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


        [Obsolete("This property is Obsolete. Please use `SkipSavingLogToApplicationDatabase`. This property will be removed in future versions")]
        public bool SkipSaving
        {
            get { return SkipSavingLogToApplicationDatabase; }
            set { SkipSavingLogToApplicationDatabase = value; }
        }

        public bool SkipSavingLogToApplicationDatabase { get; set; } = false;

        public bool SkipSavingLogToSerilog { get; set; } = false;
    }
}
