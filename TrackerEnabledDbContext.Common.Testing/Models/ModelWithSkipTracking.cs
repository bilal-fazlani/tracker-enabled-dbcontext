using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class ModelWithSkipTracking
    {
        public int Id { get; set; }
        public Guid TrackedProperty { get; set; }

        [SkipTracking]
        public string UnTrackedProperty { get; set; }
    }
}
