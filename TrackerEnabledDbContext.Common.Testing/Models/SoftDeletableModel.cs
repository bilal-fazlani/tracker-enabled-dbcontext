using System.ComponentModel.DataAnnotations;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class SoftDeletableModel :ISoftDeletable
    {
        public long Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
