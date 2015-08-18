using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class ChildModel
    {
        [Key]
        public int Id { get; set; }

        public int ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual ParentModel Parent { get; set; }
    }
}