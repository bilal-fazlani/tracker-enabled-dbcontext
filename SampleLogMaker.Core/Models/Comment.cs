using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SampleLogMaker.Core.Models
{
    [TrackChanges]
    public class Comment
    {
        [SkipTracking]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        public string Description { get; set; }

        public Blog Blog { get; set; }
    }
}
