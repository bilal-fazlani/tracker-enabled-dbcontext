using System.ComponentModel.DataAnnotations;

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
        public string Text { get; set; }

        [UIHint("blogselecter")]
        [Required]
        [Display(Name = "Blog")]
        public virtual Blog ParentBlog { get; set; }
    }
}