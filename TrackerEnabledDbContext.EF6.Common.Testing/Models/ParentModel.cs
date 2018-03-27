using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackerEnabledDbContext.EF6.Common.Testing.Models
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