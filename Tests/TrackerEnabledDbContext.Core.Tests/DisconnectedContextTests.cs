using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Tests;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Tests;
using TrackerEnabledDbContext.Core.Common.Tests;
using TrackerEnabledDbContext.Core.Common.Tests.Extensions;
using TrackerEnabledDbContext.Common.Tests.Code;
using System;

namespace TrackerEnabledDbContext.EF6.Tests
{
    [TestClass]
    public class DisconnectedContextTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();

        [TestInitialize]
        public void InitializeTest()
        {
            GlobalTrackingConfig.DisconnectedContext = true;
        }

        [TestMethod]
        public void Should_Be_Able_To_Update()
        {
            NormalModel entity = new NormalModel();

            NormalModel newEntity = new NormalModel
            {
                Id = entity.Id,
                Description = rdg.Get<string>()
            };

            using (TestTrackerContext newContext2 = GetNewContextInstance())
            {
                newContext2.NormalModels.Attach(newEntity);
                newContext2.Entry(newEntity).State = EntityState.Modified;
                newContext2.SaveChanges();

                newEntity.AssertAuditForModification(newContext2, newEntity.Id, null,
                    new AuditLogDetail
                    {
                        NewValue = newEntity.Description,
                        OriginalValue = entity.Description,
                        PropertyName = nameof(NormalModel.Description)
                    });
            }
        }

        [TestMethod]
        public void Should_Be_Soft_Deleted()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>(x => x.IsDeleted);

            SoftDeletableModel entity = new SoftDeletableModel();

            SoftDeletableModel newEntity = new SoftDeletableModel
            {
                Id = entity.Id,
                Description = entity.Description
            };

            using (TestTrackerContext newContext2 = GetNewContextInstance())
            {
                newEntity.Delete();
                newContext2.Entry(newEntity).State = EntityState.Modified;
                newContext2.SaveChanges();

                newEntity.AssertAuditForSoftDeletion(newContext2, newEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(entity.IsDeleted),
                        OriginalValue = false.ToString(),
                        NewValue = true.ToString()
                    });
            }
        }

        [TestMethod]
        public void should_update_with_no_logs()
        {
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

            TrackedModelWithMultipleProperties newEntity = new TrackedModelWithMultipleProperties
            {
                Id = 0,
                Description = string.Empty,
                IsSpecial = false,
                Name = string.Empty,
                StartDate = DateTime.Now,
                Value = 0,
                Category = 'C'
            };

            using (TestTrackerContext newContext2 = GetNewContextInstance())
            {
                newContext2.TrackedModelWithMultipleProperties.Attach(newEntity);
                newContext2.Entry(newEntity).State = EntityState.Modified;
                newContext2.SaveChanges();

                newEntity.AssertNoLogs(newContext2, newEntity.Id, EventType.Modified);
            }
        }

        [TestMethod]
        public void Should_Be_able_to_insert()
        {
            var entity = new NormalModel
            {
                Description = rdg.Get<string>()
            };

            using (TestTrackerContext ttc = GetNewContextInstance())
            {
                ttc.NormalModels.Attach(entity);
                ttc.Entry(entity).State = EntityState.Added;

                ttc.SaveChanges();

                entity.AssertAuditForAddition(ttc, entity.Id, null,
                    x => x.Id,
                    x => x.Description);
            }
        }

        [TestMethod]
        public void should_be_able_to_delete()
        {
            EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>()
                .Except(x=>x.IsSpecial);

            TrackedModelWithMultipleProperties existingModel = new TrackedModelWithMultipleProperties();

            var newModel = new TrackedModelWithMultipleProperties
            {
                Id = existingModel.Id
            };

            using (TestTrackerContext newContextInstance = GetNewContextInstance())
            {

                newContextInstance.TrackedModelWithMultipleProperties.Attach(newModel);
                newContextInstance.Entry(newModel).State = EntityState.Deleted;

                newContextInstance.SaveChanges();

                existingModel.AssertAuditForDeletion(newContextInstance, newModel.Id,
                    null,
                    model => model.Id,
                    model => model.Name,
                    model => model.StartDate,
                    model => model.Value,
                    model => model.Description
                    );
            }
        }

        protected TestTrackerContext GetNewContextInstance()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            return new TestTrackerContext(options);
        }
    }
}
