using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Models
{
    public class AuditLogDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string ColumnName { get; set; }

        public string OrginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual int AuditLogId { get; set; }
        [ForeignKey("AuditLogId")]
        public virtual AuditLog Log { get; set; }
    }
}