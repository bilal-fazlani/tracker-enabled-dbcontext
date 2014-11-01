using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleLogMaker.Models;

namespace Tests
{
    [TestClass]
    public class Tracker_Tests
    {
        MyDbContext db = new MyDbContext();
        Blog blog;
        Comment comment;

        [TestInitialize]
        public void Initialize()
        {
            blog = db.CreateBlog();
            comment = db.CreateComment(blog);
        }

        [TestMethod]
        public void Can_track_row_addition()
        {

        }
    }
}
