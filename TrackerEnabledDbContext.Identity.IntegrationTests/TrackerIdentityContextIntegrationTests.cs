using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TrackerEnabledDbContext.Common.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;
using TrackerEnabledDbContext.Common.Testing.Extensions;


namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    [TestClass]
    public class TrackerIdentityContextIntegrationTests : PersistanceTests<TestTrackerIdentityContext>
    {
        private string RandomText
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        [TestMethod]
        public void Can_save_model()
        {
            var model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.SaveChanges();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_when_entity_state_changed()
        {
            var model = ObjectFactory<NormalModel>.Create();
            db.Entry(model).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
            model.Id.AssertIsNotZero();
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

        [TestMethod]
        public void Can_track_addition_when_username_provided()
        {
            var randomText = RandomText;
            var userName = RandomText;

            var normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = randomText;
            db.NormalModels.Add(normalModel);
            db.SaveChanges(userName);

            normalModel.AssertAuditForAddition(db, normalModel.Id, userName, 
                new KeyValuePair<string,string>("Description", randomText),
                new KeyValuePair<string, string>("Id", normalModel.Id.ToString())
                );
        }

        [TestMethod]
        public void Can_track_addition_when_usermane_not_provided()
        {
            var randomText = RandomText;

            var normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = randomText;
            db.NormalModels.Add(normalModel);
            db.SaveChanges();

            normalModel.AssertAuditForAddition(db, normalModel.Id, null, 
                new KeyValuePair<string,string>("Description", randomText),
                new KeyValuePair<string, string>("Id", normalModel.Id.ToString())
                );
        }

        [TestMethod]
        public void Can_track_addition_when_state_changed_directly()
        {
            var randomText = RandomText;
            var userName = RandomText;

            var model = ObjectFactory<NormalModel>.Create();
            model.Description = randomText;
            db.Entry(model).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges(userName);

            model.AssertAuditForAddition(db, model.Id, userName,
                new KeyValuePair<string, string>("Description", randomText),
                new KeyValuePair<string, string>("Id", model.Id.ToString())
                );
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            var description = RandomText;
            var userName = RandomText;

            //add
            var normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = description;
            db.NormalModels.Add(normalModel);
            db.SaveChanges(userName);


            //remove
            db.NormalModels.Remove(normalModel);
            db.SaveChanges(userName);

            normalModel.AssertAuditForDeletion(db, normalModel.Id, userName,
                new KeyValuePair<string, string>("Description", normalModel.Description),
                new KeyValuePair<string, string>("Id", normalModel.Id.ToString())
                );
        }

        [TestMethod]
        public void Can_track_deletion_when_state_changed()
        {
            var description = RandomText;

            //add
            var normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = description;
            db.NormalModels.Add(normalModel);
            db.SaveChanges();


            //remove
            db.Entry(normalModel).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();


            //assert
            normalModel.AssertAuditForDeletion(db, normalModel.Id, null,
                new KeyValuePair<string, string>("Description", normalModel.Description),
                new KeyValuePair<string, string>("Id", normalModel.Id.ToString())
                );
        }

        [TestMethod]
        public void Can_track_local_propery_change()
        {
            //add enity
            var oldDescription = RandomText;
            var newDescription = RandomText;
            var entity = new NormalModel {Description = oldDescription };
            db.Entry(entity).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();

            //modify entity
            entity.Description = newDescription;
            db.SaveChanges();

            var expectedLog = new List<AuditLogDetail> {
                new AuditLogDetail{
                    NewValue = newDescription,
                    OriginalValue = oldDescription,
                    ColumnName = "Description"
                }}.ToArray();


            //assert
            entity.AssertAuditForModification(db, entity.Id, null, expectedLog);
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            //add enitties
            var parent1 = new ParentModel();
            var child = new ChildModel { Parent = parent1 };
            db.Children.Add(child);
            db.SaveChanges();

            child.Id.AssertIsNotZero(); //assert child saved
            parent1.Id.AssertIsNotZero(); //assert parent1 saved

            //save parent 2
            var parent2 = new ParentModel();
            db.ParentModels.Add(parent2);
            db.SaveChanges();

            parent2.Id.AssertIsNotZero(); //assert parent2 saved

            //change parent
            child.Parent = parent2;
            db.SaveChanges();

            var expectedLog = new List<AuditLogDetail> {
                new AuditLogDetail{
                    NewValue = parent2.Id.ToString(),
                    OriginalValue = parent1.Id.ToString(),
                    ColumnName = "ParentId"
                }}.ToArray();

            //assert change
            child.AssertAuditForModification(db, child.Id, null, expectedLog);
        }
    }
}
