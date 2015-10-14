using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
