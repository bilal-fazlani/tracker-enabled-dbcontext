using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleLogMaker.Models
{
    [TrackChanges]
    public class Blog
    {
        public Blog()
        {
            Comments = new List<Comment>();
        }

        [Key]
        [SkipTracking]
        [UIHint("int")]
        public int Id { get; set; }

        [Required]
        [UIHint("string")]
        public string Title { get; set; }

        [UIHint("textarea")]
        [Column("BlogContent")]
        public string Description { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }
}