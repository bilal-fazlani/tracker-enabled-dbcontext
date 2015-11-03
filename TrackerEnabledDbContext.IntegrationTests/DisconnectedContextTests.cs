using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class DisconnectedContextTests : PersistanceTests<TestTrackerContext>
    {
        [TestInitialize]
        public void InitializeTest()
        {
            GlobalTrackingConfig.DisconnectedContext = true;
        }

        [TestMethod]
        public void Should_Be_Able_To_Update()
        {
            var entity = GetDisconnectedExistingNormalModel();

            NormalModel newEntity = new NormalModel
            {
                Id = entity.Id,
                Description = RandomText
            };

            TestTrackerContext newContext2 = GetNewContextInstance();
            newContext2.NormalModels.Attach(newEntity);
            newContext2.Entry(newEntity).State = EntityState.Modified;
            newContext2.SaveChanges();

            newEntity.AssertAuditForModification(newContext2, newEntity.Id, null,
                new AuditLogDetail
                {
                    NewValue = newEntity.Description,
                    OriginalValue = null,
                    PropertyName = nameof(NormalModel.Description)
                });
        }

        [TestMethod]
        public void should_update_with_no_logs()
        {
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            var entity = GetDisconnectedExistingComplexModel();

            TrackedModelWithMultipleProperties newEntity = new TrackedModelWithMultipleProperties
            {
                Id = entity.Id,
                Description = entity.Description,
                IsSpecial = entity.IsSpecial,
                Name = entity.Name,
                StartDate = entity.StartDate,
                Value = entity.Value,
                Category = entity.Category
            };

            TestTrackerContext newContext2 = GetNewContextInstance();
            newContext2.TrackedModelsWithMultipleProperties.Attach(newEntity);
            newContext2.Entry(newEntity).State = EntityState.Modified;
            newContext2.SaveChanges();

            newEntity.AssertNoLogs(newContext2, newEntity.Id, EventType.Modified);
        }

        [TestMethod]
        public void Should_Be_able_to_insert()
        {
            var entity = new NormalModel
            {
                Description = RandomText
            };

            db.NormalModels.Attach(entity);
            db.Entry(entity).State = EntityState.Added;

            db.SaveChanges();

            entity.AssertAuditForAddition(db, entity.Id, null,
                x => x.Id,
                x => x.Description);
        }

        [TestMethod]
        public void should_be_able_to_delete()
        {
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();
            TrackedModelWithMultipleProperties existingModel = GetDisconnectedExistingComplexModel();

            TestTrackerContext newContextInstance = GetNewContextInstance();

            var newModel = new TrackedModelWithMultipleProperties
            {
                Id = existingModel.Id
            };

            newContextInstance.TrackedModelsWithMultipleProperties.Attach(newModel);
            newContextInstance.Entry(newModel).State = EntityState.Deleted;

            newContextInstance.SaveChanges();

            existingModel.AssertAuditForDeletion(newContextInstance, newModel.Id,
                null, 
                model => model.Id,
                model=> model.IsSpecial,
                model => model.Name,
                model => model.StartDate,
                model => model.Value,
                model => model.Description
                );
        }

        private NormalModel GetDisconnectedExistingNormalModel()
        {
            TestTrackerContext newContext1 = GetNewContextInstance();
            NormalModel entity = new NormalModel();
            newContext1.NormalModels.Add(entity);
            newContext1.SaveChanges();

            entity.Id.AssertIsNotZero("entity was not saved to database");

            return entity;
        }

        private TrackedModelWithMultipleProperties GetDisconnectedExistingComplexModel()
        {
            TestTrackerContext newContext1 = GetNewContextInstance();
            TrackedModelWithMultipleProperties entity = new TrackedModelWithMultipleProperties
            {
                Description = RandomText,
                IsSpecial = true,
                Name = RandomText,
                StartDate = RandomDate,
                Value = RandomNumber,
                Category = RandomChar
            };
            newContext1.TrackedModelsWithMultipleProperties.Add(entity);
            newContext1.SaveChanges();

            entity.Id.AssertIsNotZero("entity was not saved to database");

            return entity;
        }

        private TestTrackerContext GetNewContextInstance()
        {
            return new TestTrackerContext();
        }
    }
}
