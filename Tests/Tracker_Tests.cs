using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleLogMaker.Models;
using System.Linq;
using TrackerEnabledDbContext.Common.Models;

namespace Tests
{
    [TestClass]
    public class Tracker_Tests : PersistanceTests
    {
        [TestMethod]
        public void Can_track_row_addition()
        {
            //add a blog
            var blog = ModelFactory.CreateBlog(db);

            //fetch log for it
            var log = db.GetLogs<Blog>(blog.Id).SingleOrDefault(x => x.EventType == EventType.Added);

            Assert.IsNotNull(log);
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            //create log
            var blog = ModelFactory.CreateBlog(db);
            string blogId = blog.Id.ToString();
            Assert.IsTrue(blog.Id > 0);

            //remove log
            db.Blogs.Remove(blog);
            db.SaveChanges(ModelFactory.USERNAME);

            //fetch removal log
            var log = db.GetLogs<Blog>(blogId).SingleOrDefault(x => x.RecordId == blogId && x.EventType == EventType.Deleted);
            Assert.IsNotNull(log);
        }

        [TestMethod]
        public void Can_track_local_propery_change()
        {
            //create log
            var blog = ModelFactory.CreateBlog(db);

            //change property
            var oldTitle = blog.Title;
            var newTitle = Guid.NewGuid().ToString();
            blog.Title = newTitle;
            db.SaveChanges(ModelFactory.USERNAME);

            //fetch log
            var log = db.GetLogs<Blog>(blog.Id).ToList().SingleOrDefault(x => x.RecordId == blog.Id.ToString() && x.EventType == EventType.Modified);
            Assert.IsNotNull(log);
            Assert.IsTrue(log.LogDetails.Any(x => x.ColumnName == "Title" && x.NewValue == newTitle && x.OriginalValue == oldTitle));
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            var blog1 = db.CreateBlog();
            var comment = db.CreateComment(blog1);

            var blog2 = db.CreateBlog();
            comment.ParentBlog = blog2;

            var state = db.Entry<Comment>(comment);

            db.SaveChanges(ModelFactory.USERNAME);

            //fetch log
            var logPresent = db.GetLogs<Comment>(comment.Id).ToList()
                .Any(x=>x.EventType == EventType.Modified);
            Assert.IsTrue(logPresent);
        }
    }
}
