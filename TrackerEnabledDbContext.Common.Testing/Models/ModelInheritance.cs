using System;
using System.ComponentModel.DataAnnotations;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public DateTime Modified { get; set; }

        [SkipTracking]
        public string UntrackedProperty { get; set; }
    }

    public class ExtendedModel : BaseModel
    {
        public string TrackedProperty { get; set; }
    }

    [TrackChanges]
    public class TrackedExtendedModel : BaseModel
    {
    }
}
