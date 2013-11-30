using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext
{
    /// <summary>
    /// This model class is used to store the changes made in datbase values
    /// For the audit purpose. Only selected tables can be tracked with the help of TrackChanges Attribute present in the common library.
    /// </summary>
    public class AuditLog
    {
        public AuditLog()
        {
            Children = new List<AuditLogChild>();
        }

        [Key]
        public Guid AuditLogID { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTimeOffset EventDateUTC { get; set; }

        [Required]
        [MaxLength(1)]
        public string EventType { get; set; }

        [Required]
        [MaxLength(256)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(256)]
        public string RecordID { get; set; }

        [Required]
        [MaxLength(256)]
        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public virtual ICollection<AuditLogChild> Children { get; set; }

    }

    public class AuditLogChild
    {
        [Key]
        public int Id { get; set; }

        public virtual Guid AuditLogId { get; set; }
        
        [ForeignKey("AuditLogId")]
        public virtual AuditLog Log { get; set; }
        
        [Required]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
