using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class ParentModel
    {
        public ParentModel()
        {
            Children = new List<ChildModel>();
        }

        [Key]
        public int Id { get; set; }
        public virtual List<ChildModel> Children { get; set; } 
    }
}
