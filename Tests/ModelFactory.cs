using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Models;
using TrackerEnabledDbContext;

namespace Tests
{
    public static class ModelFactory
    {
        public const string USERNAME = "unit test";

        //public static Blog CreateBlog(this TestTrackerContext context)
        //{
        //    Blog b = new Blog
        //    {
        //        Description = RandomString(),
        //        Title = RandomString()
        //    };

        //    context.Blogs.Add(b);
        //    context.SaveChanges(USERNAME);
        //    return b;
        //}

        //public static Comment CreateComment(this TestTrackerContext context, Blog blog)
        //{
        //    Comment c = new Comment
        //    {
        //        Text = RandomString(),
        //        ParentBlog = blog
        //    };

        //    context.Comments.Add(c);
        //    context.SaveChanges(USERNAME);
        //    return c;
        //}

        //public static async Task<Blog> CreateBlogAsync(this TestTrackerContext context)
        //{
        //    Blog b = new Blog
        //    {
        //        Description = RandomString(),
        //        Title = RandomString()
        //    };

        //    context.Blogs.Add(b);
        //    await context.SaveChangesAsync(USERNAME);
        //    return b;
        //}

        //public static Car CreateCar(this TestTrackerContext context)
        //{
        //    var car = new Car {
        //        Description = RandomString(),
        //        Make = RandomString(),
        //        ModelName = RandomString()
        //    };
        //    context.Cars.Add(car);
        //    context.SaveChanges();
        //    return car;
        //}

        private static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
