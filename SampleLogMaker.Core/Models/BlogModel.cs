using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleLogMaker.Core.Models
{
    [TrackChanges]
    public class Blog
    {
        [Key]
        [SkipTracking]
        public int Id { get; set; }

        public string Title { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
    }
}
