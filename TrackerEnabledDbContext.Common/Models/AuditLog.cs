using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackerEnabledDbContext.Common.Interfaces;

namespace TrackerEnabledDbContext.Common.Models
{
    /// <summary>
    ///     This model class is used to store the changes made in datbase values
    ///     For the audit purpose. Only selected tables can be tracked with the help of TrackChangesAttribute Attribute present
    ///     in the common library.
    /// </summary>
    public class AuditLog: IUnTrackable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditLogId { get; set; }

        public string UserName { get; set; }

        [Required]
        public DateTime EventDateUTC { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        [MaxLength(512)]
        public string TypeFullName { get; set; }

        [Required]
        [MaxLength(256)]
        public string RecordId { get; set; }

        public virtual ICollection<AuditLogDetail> LogDetails { get; set; } = new List<AuditLogDetail>();

        public virtual ICollection<LogMetadata> Metadata { get; set; } = new List<LogMetadata>();
    }
}