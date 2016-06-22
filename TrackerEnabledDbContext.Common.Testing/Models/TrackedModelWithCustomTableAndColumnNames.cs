using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [Table("ModelWithCustomTableAndColumnNames")]
    public class TrackedModelWithCustomTableAndColumnNames
    {
        public long Id { get; set; }

        [Column("MagnitudeOfForce")]
        public int Magnitude { get; set; }

        public string Direction { get; set; }

        public string Subject { get; set; }
    }
}
