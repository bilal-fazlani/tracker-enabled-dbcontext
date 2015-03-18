using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TrackerEnabledDbContext.Common.Models;
using Tests.Extensions;
using Tests.Models;
using System.Threading.Tasks;
using static Tests.Extensions.AssertExtensions;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class TrackerContextIntegrationTests : PersistanceTests
    {
        const string USERNAME = "integrationTests";

        [TestMethod]
        public void Can_save_model()
        {
            var model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.SaveChanges();
            AssertExtensions.AssertIsNotZero(model.Id);
        }

        [TestMethod]
        public void Can_save_when_entity_state_changed()
        {
            var model = ObjectFactory<NormalModel>.Create();
            db.Entry(model).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
            AssertExtensions.AssertIsNotZero(model.Id);
        }

        [TestMethod]
        public async Task Can_save_async()
        {
            var model = ObjectFactory<NormalModel>.Create();
            db.Entry(model).State = System.Data.Entity.EntityState.Added;
            await db.SaveChangesAsync();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent()
        {
            var child = new ChildModel();
            var parent = new ParentModel();
            child.Parent = parent;

            db.Children.Add(child);

            db.SaveChanges();

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent_when_entity_state_changed()
        {
            var child = new ChildModel();
            var parent = new ParentModel();
            child.Parent = parent;

            db.Entry(child).State = System.Data.Entity.EntityState.Added;

            db.SaveChanges();

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        //[TestMethod]
        //public void Can_track_row_addition()
        //{
        //    //add a blog
        //    var blog = ModelFactory.CreateBlog(db);

        //    //fetch log for it
        //    var log = db.GetLogs<Blog>(blog.Id).SingleOrDefault(x => x.EventType == EventType.Added);

        //    Assert.IsNotNull(log);
        //}

        //[TestMethod]
        //public void Can_track_deletion()
        //{
        //    //create log
        //    var blog = ModelFactory.CreateBlog(db);
        //    string blogId = blog.Id.ToString();
        //    Assert.IsTrue(blog.Id > 0);

        //    //remove log
        //    db.Blogs.Remove(blog);
        //    db.SaveChanges(ModelFactory.USERNAME);

        //    //fetch removal log
        //    var log = db.GetLogs<Blog>(blogId).SingleOrDefault(x => x.RecordId == blogId && x.EventType == EventType.Deleted);
        //    Assert.IsNotNull(log);
        //}

        //[TestMethod]
        //public void Can_track_local_propery_change()
        //{
        //    //create log
        //    var blog = ModelFactory.CreateBlog(db);

        //    //change property
        //    var oldTitle = blog.Title;
        //    var newTitle = Guid.NewGuid().ToString();
        //    blog.Title = newTitle;
        //    db.SaveChanges(ModelFactory.USERNAME);

        //    //fetch log
        //    var log = db.GetLogs<Blog>(blog.Id).ToList().SingleOrDefault(x => x.RecordId == blog.Id.ToString() && x.EventType == EventType.Modified);
        //    Assert.IsNotNull(log);
        //    Assert.IsTrue(log.LogDetails.Any(x => x.ColumnName == "Title" && x.NewValue == newTitle && x.OriginalValue == oldTitle));
        //}

        //[TestMethod]
        //public void Can_track_navigational_property_change()
        //{
        //    var blog1 = db.CreateBlog();
        //    var comment = db.CreateComment(blog1);

        //    var blog2 = db.CreateBlog();
        //    comment.ParentBlog = blog2;

        //    var state = db.Entry<Comment>(comment);

        //    db.SaveChanges(ModelFactory.USERNAME);

        //    //fetch log
        //    var logPresent = db.GetLogs<Comment>(comment.Id).ToList()
        //        .Any(x=>x.EventType == EventType.Modified);
        //    Assert.IsTrue(logPresent);
        //}

        //[TestMethod]
        //public void Can_save_with_conventional_key()
        //{
        //    var car = db.CreateCar();
        //    Assert.AreNotEqual(0, car.Id);
        //}

        //[TestMethod]
        //public void Can_get_logs()
        //{

        //}
    }
}
