using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleLogMaker.Models;

namespace Tests
{
    [TestClass]
    public class CRUD_Tests : PersistanceTests
    {
        Blog b;
        Comment c;

        [TestInitialize]
        public void LocalInitialise()
        {
            b = db.CreateBlog();
            c = db.CreateComment(b);
        }

        [TestMethod]
        public void CanSaveBlog()
        {
            Assert.IsTrue(b.Id > 0);
        }

        [TestMethod]
        public void Can_SaveComment()
        {
            Assert.IsTrue(c.Id > 0);
        }

        [TestMethod]
        public void Can_Fetch_Comment_and_its_blog()
        {
            var comment = db.Comments.Find(c.Id);

            Assert.IsNotNull(comment);
            Assert.IsNotNull(comment.ParentBlog);
        }

        [TestMethod]
        public void Can_save_recursively()
        {
            var blog = new Blog {Description = "abc",Title ="qwe" };

            var comment = new Comment { ParentBlog = blog, Text = "tyu" };
            db.Comments.Add(comment);

            db.SaveChanges("unit test");

            Assert.IsTrue(blog.Id > 0);
            Assert.IsTrue(comment.Id > 0);
        }
    }
}
