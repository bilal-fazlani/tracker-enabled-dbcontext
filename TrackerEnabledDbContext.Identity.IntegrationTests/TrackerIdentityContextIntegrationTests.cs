using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Auditors;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    [TestClass]
    public class TrackerIdentityContextIntegrationTests : PersistanceTests<TestTrackerIdentityContext>
    {
        [TestMethod]
        public void Can_save_model()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.SaveChanges();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_when_entity_state_changed()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.Entry(model).State = EntityState.Added;
            db.SaveChanges();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public async Task Can_save_async()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.Entry(model).State = EntityState.Added;
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

            db.Entry(child).State = EntityState.Added;

            db.SaveChanges();

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_track_addition_when_username_provided()
        {
            string randomText = RandomText;
            string userName = RandomText;

            NormalModel normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = randomText;
            db.NormalModels.Add(normalModel);
            db.SaveChanges(userName);

            normalModel.AssertAuditForAddition(db, normalModel.Id, userName,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_addition_when_usermane_not_provided()
        {
            string randomText = RandomText;

            NormalModel normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = randomText;
            db.NormalModels.Add(normalModel);
            db.SaveChanges();

            normalModel.AssertAuditForAddition(db, normalModel.Id, null,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_addition_when_state_changed_directly()
        {
            string randomText = RandomText;
            string userName = RandomText;

            NormalModel model = ObjectFactory<NormalModel>.Create();
            model.Description = randomText;
            db.Entry(model).State = EntityState.Added;
            db.SaveChanges(userName);

            model.AssertAuditForAddition(db, model.Id, userName,
                 x => x.Description,
                 x => x.Id);
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            string description = RandomText;
            string userName = RandomText;

            //add
            NormalModel normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = description;
            db.NormalModels.Add(normalModel);
            db.SaveChanges(userName);


            //remove
            db.NormalModels.Remove(normalModel);
            db.SaveChanges(userName);

            normalModel.AssertAuditForDeletion(db, normalModel.Id, userName,
                 x => x.Description,
                 x => x.Id);
        }

        [TestMethod]
        public void Can_track_deletion_when_state_changed()
        {
            string description = RandomText;

            //add
            NormalModel normalModel = ObjectFactory<NormalModel>.Create();
            normalModel.Description = description;
            db.NormalModels.Add(normalModel);
            db.SaveChanges();


            //remove
            db.Entry(normalModel).State = EntityState.Deleted;
            db.SaveChanges();


            //assert
            normalModel.AssertAuditForDeletion(db, normalModel.Id, null,
                 x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_local_propery_change()
        {
            //add enity
            string oldDescription = RandomText;
            string newDescription = RandomText;
            var entity = new NormalModel {Description = oldDescription};
            db.Entry(entity).State = EntityState.Added;
            db.SaveChanges();

            //modify entity
            entity.Description = newDescription;
            db.SaveChanges();

            AuditLogDetail[] expectedLog = new List<AuditLogDetail>
            {
                new AuditLogDetail
                {
                    NewValue = newDescription,
                    OriginalValue = oldDescription,
                    PropertyName = "Description"
                }
            }.ToArray();


            //assert
            entity.AssertAuditForModification(db, entity.Id, null, expectedLog);
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            //add enitties
            var parent1 = new ParentModel();
            var child = new ChildModel {Parent = parent1};
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

            AuditLogDetail[] expectedLog = new List<AuditLogDetail>
            {
                new AuditLogDetail
                {
                    NewValue = parent2.Id.ToString(),
                    OriginalValue = parent1.Id.ToString(),
                    PropertyName = "ParentId"
                }
            }.ToArray();

            //assert change
            child.AssertAuditForModification(db, child.Id, null, expectedLog);
        }

        [TestMethod]
        public async Task Can_skip_tracking_of_property()
        {
            string username = RandomText;

            //add enitties
            var entity = new ModelWithSkipTracking {TrackedProperty = Guid.NewGuid(), UnTrackedProperty = RandomText};
            db.ModelsWithSkipTracking.Add(entity);
            await db.SaveChangesAsync(username, CancellationToken.None);

            //assert enity added
            entity.Id.AssertIsNotZero();

            //assert addtion
            entity.AssertAuditForAddition(db, entity.Id, username,
                 x => x.TrackedProperty,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_composite_keys()
        {
            string key1 = RandomText;
            string key2 = RandomText;
            string userName = RandomText;
            string descr = RandomText;


            ModelWithCompositeKey entity = ObjectFactory<ModelWithCompositeKey>.Create();
            entity.Description = descr;
            entity.Key1 = key1;
            entity.Key2 = key2;

            db.ModelsWithCompositeKey.Add(entity);
            db.SaveChanges(userName);

            string expectedKey = string.Format("[{0},{1}]", key1, key2);

            entity.AssertAuditForAddition(db, expectedKey, userName,
                x=>x.Description,
                x=>x.Key1,
                x=>x.Key2
                );
        }

        [TestMethod]
        public async Task Can_get_logs_by_table_name()
        {
            string descr = RandomText;
            NormalModel model = ObjectFactory<NormalModel>.Create();
            model.Description = descr;

            db.NormalModels.Add(model);
            await db.SaveChangesAsync(CancellationToken.None);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = db.GetLogs("TrackerEnabledDbContext.Common.Testing.Models.NormalModel", model.Id)
                .AssertCountIsNotZero("logs not found");

            AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

            IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                .AssertIsNotNull("log details is null")
                .AssertCountIsNotZero("no log details found");
        }

        [TestMethod]
        public async Task Can_get_logs_by_entity_type()
        {
            string descr = RandomText;
            NormalModel model = ObjectFactory<NormalModel>.Create();
            model.Description = descr;

            db.NormalModels.Add(model);
            await db.SaveChangesAsync(CancellationToken.None);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = db.GetLogs<NormalModel>(model.Id)
                .AssertCountIsNotZero("logs not found");

            AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

            IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                .AssertIsNotNull("log details is null")
                .AssertCountIsNotZero("no log details found");
        }

        [TestMethod]
        public async Task Can_get_all_logs()
        {
            string descr = RandomText;
            NormalModel model = ObjectFactory<NormalModel>.Create();
            model.Description = descr;

            db.NormalModels.Add(model);
            await db.SaveChangesAsync(RandomText);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = db
                .GetLogs("TrackerEnabledDbContext.Common.Testing.Models.NormalModel")
                .ToList();

            logs.AssertCountIsNotZero("logs not found");

            AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

            IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                .AssertIsNotNull("log details is null");

            details.AssertCountIsNotZero("no log details found");
        }

        [TestMethod]
        public async Task Can_save_changes_with_userID()
        {
            int userId = RandomNumber;

            //add enity
            string oldDescription = RandomText;
            string newDescription = RandomText;
            var entity = new NormalModel {Description = oldDescription};
            db.Entry(entity).State = EntityState.Added;
            db.SaveChanges(userId);

            //modify entity
            entity.Description = newDescription;
            await db.SaveChangesAsync(userId);

            //assert
            entity.AssertAuditForModification(db, entity.Id, userId.ToString(), new AuditLogDetail
            {
                    NewValue = newDescription,
                    OriginalValue = oldDescription,
                    PropertyName = "Description"
            });
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForAddedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.ChangeTracker.DetectChanges();
            var entry = db.ChangeTracker.Entries().First();
            var auditor = new AdditionLogDetailsAuditor(entry, null);

            db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            db.Database.Log = null;
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForModifiedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.SaveChanges();
            model.Description += RandomText;
            db.ChangeTracker.DetectChanges();
            var entry = db.ChangeTracker.Entries().First();
            var auditor = new ChangeLogDetailsAuditor(entry, null);

            db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            db.Database.Log = null;
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForDeletedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory<NormalModel>.Create();
            db.NormalModels.Add(model);
            db.SaveChanges();
            db.NormalModels.Remove(model);
            db.ChangeTracker.DetectChanges();
            var entry = db.ChangeTracker.Entries().First();
            var auditor = new ChangeLogDetailsAuditor(entry, null);

            db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            db.Database.Log = null;
        }
    }
}