using System;

namespace TrackerEnabledDbContext.Common.Tools
{
    public class NameChangedEventArgs : EventArgs
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
        public long RecordId { get; set; }
    }
}
