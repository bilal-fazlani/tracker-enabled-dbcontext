using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Common.Models
{
    public class AuditLogDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string PropertyName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual int AuditLogId { get; set; }

        [ForeignKey("AuditLogId")]
        public virtual AuditLog Log { get; set; }
    }
}