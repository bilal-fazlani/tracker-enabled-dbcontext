using System;
using System.Collections.Generic;
using TrackingCore.Models;

namespace TrackingCore.Events
{
    public class AuditLogGeneratedEventArgs : EventArgs
    {
        public object Entity { get; internal set; }

        public dynamic Metadata { get; internal set; }

        public DateTime Date { get; internal set; }

        public EventType EventType { get; set; }

        public object EntityId { get; internal set; }

        public List<AuditLogDetail> LogDetails { get; internal set; } = new List<AuditLogDetail>();

        public object Username { get; internal set; }
    }
}
