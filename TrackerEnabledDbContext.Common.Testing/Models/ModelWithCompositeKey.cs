using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerEnabledDbContext.Common.Testing.Models
{
    [TrackChanges]
    public class ModelWithCompositeKey
    {
        [Key, Column(Order = 1)]
        public string Key1 { get; set; }

        [Key, Column(Order = 2)]
        public string Key2 { get; set; }

        public string Description { get; set; }
    }
}