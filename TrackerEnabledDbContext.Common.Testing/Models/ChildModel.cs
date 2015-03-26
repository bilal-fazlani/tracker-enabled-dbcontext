using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
