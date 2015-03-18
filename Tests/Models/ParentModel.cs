using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    [TrackChanges]
    public class ParentModel
    {
        [Key]
        public int Id { get; set; }
        public virtual List<ChildModel> Children { get; set; } = new List<ChildModel>();
    }
}
