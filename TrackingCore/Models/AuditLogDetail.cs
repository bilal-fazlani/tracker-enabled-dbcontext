using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackingCore.Interfaces;

namespace TrackingCore.Models
{
    public class AuditLogDetail 
    {
        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }
    }
}