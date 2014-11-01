using SampleLogMaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerEnabledDbContext;

namespace Tests
{
    public static class ModelFactory
    {
        public static Blog CreateBlog(this MyDbContext context)
        {
            Blog b = new Blog
            {
                Description = "test",
                Title = "test"
            };

            context.Blogs.Add(b);
            context.SaveChanges("unit test");
            return b;
        }

        public static Comment CreateComment(this MyDbContext context, Blog blog)
        {
            Comment c = new Comment
            {
                Text = "text",
                ParentBlog = blog
            };

            context.Comments.Add(c);
            context.SaveChanges("unit test");
            return c;
        }
    }
}
