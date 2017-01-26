using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Auditors;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class TrackerContextIntegrationTests : PersistanceTests<TestTrackerContext>
    {
        [TestMethod]
        public void Can_save_model()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_when_entity_state_changed()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.Entry(model).State = EntityState.Added;
            Db.SaveChanges();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public async Task Can_save_async()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.Entry(model).State = EntityState.Added;
            await Db.SaveChangesAsync();
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent()
        {
            var child = new ChildModel();
            var parent = new ParentModel();
            child.Parent = parent;

            Db.Children.Add(child);

            Db.SaveChanges();

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent_when_entity_state_changed()
        {
            var child = new ChildModel();
            var parent = new ParentModel();
            child.Parent = parent;

            Db.Entry(child).State = EntityState.Added;

            Db.SaveChanges();

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_track_addition_when_username_provided()
        {
            string randomText = RandomText;
            string userName = RandomText;

            NormalModel normalModel = ObjectFactory.Create<NormalModel>();
            normalModel.Description = randomText;
            Db.NormalModels.Add(normalModel);
            Db.SaveChanges(userName);

            normalModel.AssertAuditForAddition(Db, normalModel.Id, userName,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_addition_when_usermane_not_provided()
        {
            string randomText = RandomText;

            NormalModel normalModel = ObjectFactory.Create<NormalModel>();
            normalModel.Description = randomText;
            Db.NormalModels.Add(normalModel);
            Db.SaveChanges();

            normalModel.AssertAuditForAddition(Db, normalModel.Id, null,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_addition_when_state_changed_directly()
        {
            string randomText = RandomText;
            string userName = RandomText;

            NormalModel model = ObjectFactory.Create<NormalModel>();
            model.Description = randomText;
            Db.Entry(model).State = EntityState.Added;
            Db.SaveChanges(userName);

            model.AssertAuditForAddition(Db, model.Id, userName,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            string description = RandomText;
            string userName = RandomText;

            //add
            NormalModel normalModel = ObjectFactory.Create<NormalModel>();
            normalModel.Description = description;
            Db.NormalModels.Add(normalModel);
            Db.SaveChanges(userName);


            //remove
            Db.NormalModels.Remove(normalModel);
            Db.SaveChanges(userName);

            normalModel.AssertAuditForDeletion(Db, normalModel.Id, userName,
                x => x.Description,
                x => x.Id);
        }

        [TestMethod]
        public void Can_track_deletion_when_state_changed()
        {
            string description = RandomText;

            //add
            NormalModel normalModel = ObjectFactory.Create<NormalModel>();
            normalModel.Description = description;
            Db.NormalModels.Add(normalModel);
            Db.SaveChanges();


            //remove
            Db.Entry(normalModel).State = EntityState.Deleted;
            Db.SaveChanges();


            //assert
            normalModel.AssertAuditForDeletion(Db, normalModel.Id, null,
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
            Db.Entry(entity).State = EntityState.Added;
            Db.SaveChanges();

            //modify entity
            entity.Description = newDescription;
            Db.SaveChanges();

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
            entity.AssertAuditForModification(Db, entity.Id, null, expectedLog);
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            //add enitties
            var parent1 = new ParentModel();
            var child = new ChildModel {Parent = parent1};
            Db.Children.Add(child);
            Db.SaveChanges();

            child.Id.AssertIsNotZero(); //assert child saved
            parent1.Id.AssertIsNotZero(); //assert parent1 saved

            //save parent 2
            var parent2 = new ParentModel();
            Db.ParentModels.Add(parent2);
            Db.SaveChanges();

            parent2.Id.AssertIsNotZero(); //assert parent2 saved

            //change parent
            child.Parent = parent2;
            Db.SaveChanges();

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
            child.AssertAuditForModification(Db, child.Id, null, expectedLog);
        }

        [TestMethod]
        public void Can_track_complex_type_property_change()
        {
            //add enity
            string oldDescription = RandomText;
            string newDescription = RandomText;

            //only set one of the properties on the complex type
            var complexType = new ComplexType { Property1 = oldDescription };
            var entity = new ModelWithComplexType { ComplexType = complexType };

            Db.Entry(entity).State = EntityState.Added;
            Db.SaveChanges();

            //modify entity
            entity.ComplexType.Property1 = newDescription;
            Db.SaveChanges();

            AuditLogDetail[] expectedLog = new List<AuditLogDetail>
            {
                new AuditLogDetail
                {
                    NewValue = newDescription,
                    OriginalValue = oldDescription,
                    PropertyName = "ComplexType_Property1"
                }
            }.ToArray();


            //assert
            entity.AssertAuditForModification(Db, entity.Id, null, expectedLog);
        }

        [TestMethod]
        public async Task Can_skip_tracking_of_property()
        {
            string username = RandomText;

            //add enitties
            var entity = new ModelWithSkipTracking {TrackedProperty = Guid.NewGuid(), UnTrackedProperty = RandomText};
            Db.ModelsWithSkipTracking.Add(entity);
            await Db.SaveChangesAsync(username, CancellationToken.None);

            //assert enity added
            entity.Id.AssertIsNotZero();

            //assert addtion
            entity.AssertAuditForAddition(Db, entity.Id, username,
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


            ModelWithCompositeKey entity = ObjectFactory.Create<ModelWithCompositeKey>();
            entity.Description = descr;
            entity.Key1 = key1;
            entity.Key2 = key2;

            Db.ModelsWithCompositeKey.Add(entity);
            Db.SaveChanges(userName);

            string expectedKey = $"[{key1},{key2}]";

            entity.AssertAuditForAddition(Db, expectedKey, userName,
                x => x.Description,
                x => x.Key1,
                x=>x.Key2);
        }

        [TestMethod]
        public async Task Can_get_logs_by_table_name()
        {
            string descr = RandomText;
            NormalModel model = ObjectFactory.Create<NormalModel>();
            model.Description = descr;

            Db.NormalModels.Add(model);
            await Db.SaveChangesAsync(CancellationToken.None);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = Db.GetLogs("TrackerEnabledDbContext.Common.Testing.Models.NormalModel", model.Id)
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
            NormalModel model = ObjectFactory.Create<NormalModel>();
            model.Description = descr;

            Db.NormalModels.Add(model);
            await Db.SaveChangesAsync(CancellationToken.None);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = Db.GetLogs<NormalModel>(model.Id)
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
            NormalModel model = ObjectFactory.Create<NormalModel>();
            model.Description = descr;

            Db.NormalModels.Add(model);
            await Db.SaveChangesAsync(RandomText);
            model.Id.AssertIsNotZero();

            IEnumerable<AuditLog> logs = Db.GetLogs("TrackerEnabledDbContext.Common.Testing.Models.NormalModel")
                .AssertCountIsNotZero("logs not found");

            AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

            IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                .AssertIsNotNull("log details is null")
                .AssertCountIsNotZero("no log details found");
        }

        [TestMethod]
        public async Task Can_save_changes_with_userID()
        {
            int userId = RandomNumber;

            //add enity
            string oldDescription = RandomText;
            string newDescription = RandomText;
            var entity = new NormalModel {Description = oldDescription};
            Db.Entry(entity).State = EntityState.Added;
            Db.SaveChanges();

            //modify entity
            entity.Description = newDescription;
            await Db.SaveChangesAsync(userId);

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
            entity.AssertAuditForModification(Db, entity.Id, userId.ToString(), expectedLog);
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForAddedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.ChangeTracker.DetectChanges();
            var entry = Db.ChangeTracker.Entries().First();
            var auditor = new AdditionLogDetailsAuditor(entry, null);

            Db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            Db.Database.Log = null;
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForModifiedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            model.Description += RandomText;
            Db.ChangeTracker.DetectChanges();
            var entry = Db.ChangeTracker.Entries().First();
            var auditor = new ChangeLogDetailsAuditor(entry, null);

            Db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            Db.Database.Log = null;
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForDeletedEntity_WithoutQueryingDatabase()
        {
            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);
            Db.SaveChanges();
            Db.NormalModels.Remove(model);
            Db.ChangeTracker.DetectChanges();
            var entry = Db.ChangeTracker.Entries().First();
            var auditor = new ChangeLogDetailsAuditor(entry, null);

            Db.Database.Log = sql => Assert.Fail("Expected no database queries but the following query was executed: {0}", sql);
            var auditLogDetails = auditor.CreateLogDetails().ToList();
            Db.Database.Log = null;
        }

        [TestMethod]
        public void Should_Not_Log_When_Value_Not_changed()
        {
            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

            string oldDescription = RandomText;

            var entity = new TrackedModelWithMultipleProperties()
            {
                Description = oldDescription,
                StartDate = RandomDate,
            };
            Db.TrackedModelsWithMultipleProperties.Add(entity);
            Db.SaveChanges();

            entity.AssertAuditForAddition(Db, entity.Id,
                null,
                x => x.Id,
                x => x.Description,
                x => x.StartDate);

            //make change to state
            Db.Entry(entity).State = EntityState.Modified;
            Db.SaveChanges();

            //make sure there are no unnecessaary logs
            entity.AssertNoLogs(Db, entity.Id, EventType.Modified);
        }

        [TestMethod]
        public void Shoud_Not_Log_EmptyProperties_OnAddition()
        {
            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            var entity = new TrackedModelWithMultipleProperties();

            Db.TrackedModelsWithMultipleProperties.Add(entity);

            //act
            Db.SaveChanges();

            //assert
            entity.AssertAuditForAddition(Db, entity.Id, null,
                x => x.Id);
        }

        [TestMethod]
        public void Shoud_Not_Log_EmptyProperties_On_Deletions()
        {
            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            var entity = new TrackedModelWithMultipleProperties();
            Db.TrackedModelsWithMultipleProperties.Add(entity);
            Db.SaveChanges();

            //act (delete)
            Db.TrackedModelsWithMultipleProperties.Remove(entity);
            Db.SaveChanges();

            //assert
            entity.AssertAuditForDeletion(Db, entity.Id, null,
                x => x.Id);
        }

        [TestMethod]
        public void Should_Log_EmptyProperties_When_Configured_WhileAdding()
        {
            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = true;

            var entity = new TrackedModelWithMultipleProperties();
            Db.TrackedModelsWithMultipleProperties.Add(entity);

            //act
            Db.SaveChanges();

            //assert
            entity.AssertAuditForAddition(Db, entity.Id, null,
                x => x.Id,
                x => x.Description,
                x => x.IsSpecial,
                x => x.Name,
                x => x.StartDate,
                x => x.Value);
        }

        [TestMethod]
        public void Should_Log_EmptyProperties_When_Configured_WhileDeleting()
        {
            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = true;

            var entity = new TrackedModelWithMultipleProperties();
            Db.TrackedModelsWithMultipleProperties.Add(entity);
            Db.SaveChanges();


            //act
            Db.TrackedModelsWithMultipleProperties.Remove(entity);
            Db.SaveChanges();

            //assert
            entity.AssertAuditForDeletion(Db, entity.Id, null,
                x => x.Id,
                x => x.Description,
                x => x.IsSpecial,
                x => x.Name,
                x => x.StartDate,
                x => x.Value);
        }



        [TestMethod]
        public void Can_recognise_context_tracking_indicator_when_disabled()
        {

            NormalModel model = ObjectFactory.Create<NormalModel>();
            Db.NormalModels.Add(model);

            Db.TrackingEnabled = false;
            Db.SaveChanges();

            model.AssertNoLogs(Db, model.Id);
        }
    }
}