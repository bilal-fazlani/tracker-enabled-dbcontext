using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleLogMaker.Models
{
    [TrackChanges]
    public class Comment
    {
        [Key]
        [UIHint("int")]
        public int Id { get; set; }

        [Required]
        [UIHint("string")]
        [SkipTracking]
        public string Text { get; set; }

        public virtual int ParentBlogId { get; set; }

        [UIHint("blogselecter")]
        [Required]
        [Display(Name = "Blog")]
        [ForeignKey("ParentBlogId")]
        public virtual Blog ParentBlog { get; set; }
    }
}