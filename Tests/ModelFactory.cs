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
        public const string USERNAME = "unit test";

        public static Blog CreateBlog(this MyDbContext context)
        {
            Blog b = new Blog
            {
                Description = RandomString(),
                Title = RandomString()
            };

            context.Blogs.Add(b);
            context.SaveChanges(USERNAME);
            return b;
        }

        public static Comment CreateComment(this MyDbContext context, Blog blog)
        {
            Comment c = new Comment
            {
                Text = RandomString(),
                ParentBlog = blog
            };

            context.Comments.Add(c);
            context.SaveChanges(USERNAME);
            return c;
        }

        private static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
