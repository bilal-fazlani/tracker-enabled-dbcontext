using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Tests.Code;
using TrackerEnabledDbContext.Common.Tests.Extensions;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Common.Auditors;
using TrackerEnabledDbContext.Core.Common.Tests.Extensions;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class TrackerContextIntegrationTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();
        
        [TestMethod]
        public void Can_save_model()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
            }
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_when_entity_state_changed()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(model).State = EntityState.Added;
                ttc.SaveChanges();
            }
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public async Task Can_save_async()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(model).State = EntityState.Added;
                await ttc.SaveChangesAsync();
            }
            model.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var child = new ChildModel();
            var parent = new ParentModel();
            
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ParentModels.Add(parent);
                ttc.SaveChanges();

                child.Parent = parent;
                ttc.ChildModels.Add(child);
                ttc.SaveChanges();
            }

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_save_child_to_parent_when_entity_state_changed()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var child = new ChildModel();
            var parent = new ParentModel();
            
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ParentModels.Add(parent);
                ttc.SaveChanges();

                child.Parent = parent;

                ttc.Entry(child).State = EntityState.Added;
                ttc.SaveChanges();
            }

            child.Id.AssertIsNotZero();
            parent.Id.AssertIsNotZero();
        }

        [TestMethod]
        public void Can_track_addition_when_username_provided()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string randomText = rdg.Get<string>();
            string userName = rdg.Get<string>();

            NormalModel normalModel = new NormalModel();
            normalModel.Description = randomText;
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(normalModel);
                ttc.SaveChanges(userName);

                normalModel.AssertAuditForAddition(ttc, normalModel.Id, userName,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_addition_when_usermane_not_provided()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string randomText = rdg.Get<string>();

            NormalModel normalModel = new NormalModel();
            normalModel.Description = randomText;
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(normalModel);
                ttc.SaveChanges();

                normalModel.AssertAuditForAddition(ttc, normalModel.Id, null,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_addition_when_state_changed_directly()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string randomText = rdg.Get<string>();
            string userName = rdg.Get<string>();

            NormalModel model = new NormalModel();
            model.Description = randomText;
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(model).State = EntityState.Added;
                ttc.SaveChanges(userName);

                model.AssertAuditForAddition(ttc, model.Id, userName,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string description = rdg.Get<string>();
            string userName = rdg.Get<string>();

            //add
            NormalModel normalModel = new NormalModel();
            normalModel.Description = description;
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(normalModel);
                ttc.SaveChanges(userName);

                //remove
                ttc.NormalModels.Remove(normalModel);
                ttc.SaveChanges(userName);

                normalModel.AssertAuditForDeletion(ttc, normalModel.Id, userName,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_deletion_when_state_changed()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string description = rdg.Get<string>();

            //add
            NormalModel normalModel = new NormalModel();
            normalModel.Description = description;
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(normalModel);
                ttc.SaveChanges();

                //remove
                ttc.Entry(normalModel).State = EntityState.Deleted;
                ttc.SaveChanges();

                //assert
                normalModel.AssertAuditForDeletion(ttc, normalModel.Id, null,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_local_propery_change()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //add enity
            string oldDescription = rdg.Get<string>();
            string newDescription = rdg.Get<string>();
            var entity = new NormalModel {Description = oldDescription};
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(entity).State = EntityState.Added;
                ttc.SaveChanges();

                //modify entity
                entity.Description = newDescription;
                ttc.SaveChanges();

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
                entity.AssertAuditForModification(ttc, entity.Id, null, expectedLog);
            }
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //add enitties
            var parent1 = new ParentModel();
            var child = new ChildModel {Parent = parent1};
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ChildModels.Add(child);
                ttc.SaveChanges();

                child.Id.AssertIsNotZero(); //assert child saved
                parent1.Id.AssertIsNotZero(); //assert parent1 saved

                //save parent 2
                var parent2 = new ParentModel();
                ttc.ParentModels.Add(parent2);
                ttc.SaveChanges();

                parent2.Id.AssertIsNotZero(); //assert parent2 saved

                //change parent
                child.Parent = parent2;
                ttc.SaveChanges();

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
                child.AssertAuditForModification(ttc, child.Id, null, expectedLog);
            }
        }

        [TestMethod]
        public void Can_track_complex_type_property_change()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //add enity
            string oldDescription = rdg.Get<string>();
            string newDescription = rdg.Get<string>();

            //only set one of the properties on the complex type
            var complexType = new ComplexType { Property1 = oldDescription };
            var entity = new ModelWithComplexType { ComplexType = complexType };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(entity).State = EntityState.Added;
                ttc.SaveChanges();

                //modify entity
                entity.ComplexType.Property1 = newDescription;
                ttc.SaveChanges();

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
                entity.AssertAuditForModification(ttc, entity.Id, null, expectedLog);
            }
        }

        [TestMethod]
        public async Task Can_skip_tracking_of_property()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string username = rdg.Get<string>();

            //add enitties
            var entity = new ModelWithSkipTracking {TrackedProperty = Guid.NewGuid(), UnTrackedProperty = rdg.Get<string>()};
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ModelWithSkipTrackings.Add(entity);
                await ttc.SaveChangesAsync(username, CancellationToken.None);

                //assert enity added
                entity.Id.AssertIsNotZero();

                //assert addtion
                entity.AssertAuditForAddition(ttc, entity.Id, username,
                    x => x.TrackedProperty,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Can_track_composite_keys()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string key1 = rdg.Get<string>();
            string key2 = rdg.Get<string>();
            string userName = rdg.Get<string>();
            string descr = rdg.Get<string>();

            ModelWithCompositeKey entity = new ModelWithCompositeKey();
            entity.Description = descr;
            entity.Key1 = key1;
            entity.Key2 = key2;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ModelWithCompositeKeys.Add(entity);
                ttc.SaveChanges(userName);

                string expectedKey = $"[{key1},{key2}]";

                entity.AssertAuditForAddition(ttc, expectedKey, userName,
                    x => x.Description,
                    x => x.Key1,
                    x => x.Key2);
            }
        }

        [TestMethod]
        public async Task Can_get_logs_by_table_name()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string descr = rdg.Get<string>();
            NormalModel model = new NormalModel();
            model.Description = descr;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                await ttc.SaveChangesAsync(CancellationToken.None);
                model.Id.AssertIsNotZero();

                IEnumerable<AuditLog> logs = ttc.GetLogs("TrackerEnabledDbContext.Common.Tests.Models.NormalModel", model.Id)
                    .AssertCountIsNotZero("logs not found");

                AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

                IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                    .AssertIsNotNull("log details is null")
                    .AssertCountIsNotZero("no log details found");
            }            
        }

        [TestMethod]
        public async Task Can_get_logs_by_entity_type()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string descr = rdg.Get<string>();
            NormalModel model = new NormalModel();
            model.Description = descr;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                await ttc.SaveChangesAsync(CancellationToken.None);
                model.Id.AssertIsNotZero();

                IEnumerable<AuditLog> logs = ttc.GetLogs<NormalModel>(model.Id)
                    .AssertCountIsNotZero("logs not found");

                AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

                IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                    .AssertIsNotNull("log details is null")
                    .AssertCountIsNotZero("no log details found");
            }            
        }

        [TestMethod]
        public async Task Can_get_all_logs()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string descr = rdg.Get<string>();
            NormalModel model = new NormalModel();
            model.Description = descr;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                await ttc.SaveChangesAsync(rdg.Get<string>());
                model.Id.AssertIsNotZero();

                IEnumerable<AuditLog> logs = ttc.GetLogs("TrackerEnabledDbContext.Common.Tests.Models.NormalModel")
                    .AssertCountIsNotZero("logs not found");

                AuditLog lastLog = logs.LastOrDefault().AssertIsNotNull("last log is null");

                IEnumerable<AuditLogDetail> details = lastLog.LogDetails
                    .AssertIsNotNull("log details is null")
                    .AssertCountIsNotZero("no log details found");
            }            
        }

        [TestMethod]
        public async Task Can_save_changes_with_userID()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            int userId = rdg.Get<int>();

            //add enity
            string oldDescription = rdg.Get<string>();
            string newDescription = rdg.Get<string>();
            var entity = new NormalModel {Description = oldDescription};

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Entry(entity).State = EntityState.Added;
                ttc.SaveChanges();

                //modify entity
                entity.Description = newDescription;
                await ttc.SaveChangesAsync(userId);

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
                entity.AssertAuditForModification(ttc, entity.Id, userId.ToString(), expectedLog);
            }
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForAddedEntity_WithoutQueryingDatabase()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                ttc.ChangeTracker.DetectChanges();
                var entry = ttc.ChangeTracker.Entries().First();
                var auditor = new AdditionLogDetailsAuditor(entry, null);

                var auditLogDetails = auditor.CreateLogDetails().ToList();
            }
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForModifiedEntity_WithoutQueryingDatabase()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
                model.Description += rdg.Get<string>();
                ttc.ChangeTracker.DetectChanges();
                var entry = ttc.ChangeTracker.Entries().First();
                var auditor = new ChangeLogDetailsAuditor(entry, null);

                var auditLogDetails = auditor.CreateLogDetails().ToList();
            }
        }

        [TestMethod]
        public void Can_Create_AuditLogDetail_ForDeletedEntity_WithoutQueryingDatabase()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                ttc.SaveChanges();
                ttc.NormalModels.Remove(model);
                ttc.ChangeTracker.DetectChanges();
                var entry = ttc.ChangeTracker.Entries().First();
                var auditor = new ChangeLogDetailsAuditor(entry, null);

                var auditLogDetails = auditor.CreateLogDetails().ToList();
            }
        }

        [TestMethod]
        public void Should_Not_Log_When_Value_Not_changed()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

            string oldDescription = rdg.Get<string>();

            var entity = new TrackedModelWithMultipleProperties()
            {
                Description = oldDescription,
                StartDate = rdg.Get<DateTime>(),
            };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(entity);
                ttc.SaveChanges();

                entity.AssertAuditForAddition(ttc, entity.Id,
                    null,
                    x => x.Id,
                    x => x.Description,
                    x => x.StartDate);

                //make change to state
                ttc.Entry(entity).State = EntityState.Modified;
                ttc.SaveChanges();

                //make sure there are no unnecessaary logs
                entity.AssertNoLogs(ttc, entity.Id, EventType.Modified);
            }
        }

        [TestMethod]
        public void Shoud_Not_Log_EmptyProperties_OnAddition()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            var entity = new TrackedModelWithMultipleProperties();

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(entity);

                //act
                ttc.SaveChanges();

                //assert
                entity.AssertAuditForAddition(ttc, entity.Id, null,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Shoud_Not_Log_EmptyProperties_On_Deletions()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            var entity = new TrackedModelWithMultipleProperties();

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(entity);
                ttc.SaveChanges();

                //act (delete)
                ttc.TrackedModelWithMultipleProperties.Remove(entity);
                ttc.SaveChanges();

                //assert
                entity.AssertAuditForDeletion(ttc, entity.Id, null,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void Should_Log_EmptyProperties_When_Configured_WhileAdding()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = true;

            var entity = new TrackedModelWithMultipleProperties();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(entity);

                //act
                ttc.SaveChanges();

                //assert
                entity.AssertAuditForAddition(ttc, entity.Id, null,
                    x => x.Id,
                    x => x.Description,
                    x => x.IsSpecial,
                    x => x.Name,
                    x => x.StartDate,
                    x => x.Value);
            }
        }

        [TestMethod]
        public void Should_Log_EmptyProperties_When_Configured_WhileDeleting()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //arrange
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion = true;

            var entity = new TrackedModelWithMultipleProperties();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(entity);
                ttc.SaveChanges();

                //act
                ttc.TrackedModelWithMultipleProperties.Remove(entity);
                ttc.SaveChanges();

                //assert
                entity.AssertAuditForDeletion(ttc, entity.Id, null,
                    x => x.Id,
                    x => x.Description,
                    x => x.IsSpecial,
                    x => x.Name,
                    x => x.StartDate,
                    x => x.Value);
            }
        }

        [TestMethod]
        public void Can_recognise_context_tracking_indicator_when_disabled()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            NormalModel model = new NormalModel();
            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);

                ttc.TrackingEnabled = false;
                ttc.SaveChanges();

                model.AssertNoLogs(ttc, model.Id);
            }
        }
    }
}