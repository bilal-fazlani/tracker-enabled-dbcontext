using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrackerEnabledDbContext.Common.Models;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges(true, new [] { EventType.Deleted })]
    public class ModelWithDeleteOnlyTracking
    {
        public int Id { get; set; }
        public Guid TrackedProperty { get; set; }

        [SkipTracking]
        public string UnTrackedProperty { get; set; }
    }
}