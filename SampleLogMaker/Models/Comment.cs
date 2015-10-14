using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleLogMaker.Models
{
    
    public class Comment
    {
        [UIHint("int")]
        public int Id { get; set; }

        [UIHint("string")]
        public string Text { get; set; }

        public virtual int ParentBlogId { get; set; }

        [UIHint("blogselecter")]
        [Display(Name = "Blog")]
        public virtual Blog ParentBlog { get; set; }
    }
}