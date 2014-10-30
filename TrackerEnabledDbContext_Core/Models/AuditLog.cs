using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Models
{
    /// <summary>
    /// This model class is used to store the changes made in datbase values
    /// For the audit purpose. Only selected tables can be tracked with the help of TrackChangesAttribute Attribute present in the common library.
    /// </summary>
    public class AuditLog
    {
        public AuditLog()
        {
            LogDetails = new List<AuditLogDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuditLogId { get; set; }

        public string UserName { get; set; }

        [Required]
        public DateTimeOffset EventDateUTC { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        [MaxLength(256)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(256)]
        public string RecordId { get; set; }

        public virtual ICollection<AuditLogDetail> LogDetails { get; set; }

    }
}
