using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Common.Models
{
    using System;

    public class AuditLogDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual Guid AuditLogId { get; set; }

        [ForeignKey("AuditLogId")]
        public virtual AuditLog Log { get; set; }
    }
}