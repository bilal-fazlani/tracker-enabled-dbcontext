using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SampleLogMaker.Core.Models
{
    [TrackChanges]
    public class Blog
    {
        [SkipTracking]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        public string Title { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
