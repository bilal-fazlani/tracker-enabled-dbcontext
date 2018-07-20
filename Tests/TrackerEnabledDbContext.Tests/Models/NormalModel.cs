using System.ComponentModel.DataAnnotations;

namespace TrackerEnabledDbContext.Common.Tests.Models
{
    [TrackChanges]
    public class NormalModel
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }
    }
}