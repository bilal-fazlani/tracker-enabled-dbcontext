using System;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }

        public DateTime Modified { get; set; }
    }

    public class ExtendedModel : BaseModel
    {
        public string TrackedProperty { get; set; }
    }
}
