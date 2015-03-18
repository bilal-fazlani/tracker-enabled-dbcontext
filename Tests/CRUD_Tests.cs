using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tests.Models;
using Tests.Extensions;

namespace Tests
{
    [TestClass]
    public class CRUD_Tests : PersistanceTests
    {
        //[TestMethod]
        //public void Can_Fetch_Comment_and_its_blog()
        //{
        //    var comment = db.Comments.Find(c.Id);

        //    Assert.IsNotNull(comment);
        //    Assert.IsNotNull(comment.ParentBlog);
        //}

        //[TestMethod]
        //public void Can_save_recursively()
        //{
        //    var blog = new Blog { Description = "abc", Title = "qwe" };

        //    var comment = new Comment { ParentBlog = blog, Text = "tyu" };
        //    db.Comments.Add(comment);

        //    db.SaveChanges("unit test");

        //    Assert.IsTrue(blog.Id > 0);
        //    Assert.IsTrue(comment.Id > 0);
        //}


    }
}
