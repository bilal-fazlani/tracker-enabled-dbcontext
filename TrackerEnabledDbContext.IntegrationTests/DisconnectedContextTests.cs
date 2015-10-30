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
        [TestMethod]
        public void Should_Be_Able_To_Update()
        {
            GlobalTrackingConfig.DisconnectedContext = true;
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
        public void Should_Be_able_to_insert()
        {
            GlobalTrackingConfig.DisconnectedContext = true;

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
            NormalModel existingNormalModel = GetDisconnectedExistingNormalModel();

            TestTrackerContext newContextInstance = GetNewContextInstance();

            newContextInstance.NormalModels.Attach(existingNormalModel);
            newContextInstance.Entry(existingNormalModel).State = EntityState.Deleted;

            newContextInstance.SaveChanges();

            existingNormalModel.AssertAuditForDeletion(newContextInstance, existingNormalModel.Id,
                null, model => model.Id);
        }

        [TestMethod]
        public void Should_Not_Log_When_Value_Not_changed_In_Disconnected_Context()
        {
            //arrange
            GlobalTrackingConfig.DisconnectedContext = true;
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

            string oldDescription = RandomText;

            var entity = new TrackedModelWithMultipleProperties()
            {
                Description = oldDescription,
                StartDate = RandomDate,
            };
            db.TrackedModelsWithMultipleProperties.Add(entity);
            db.SaveChanges();

            entity.AssertAuditForAddition(db, entity.Id,
                null,
                x => x.Id,
                x => x.Description,
                x => x.StartDate);

            //make change to state
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            //make sure there are no unnecessaary logs
            entity.AssertNoLogs(db, entity.Id, EventType.Modified);
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

        private TestTrackerContext GetNewContextInstance()
        {
            return new TestTrackerContext();
        }
    }
}
