using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class NormalModel
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
