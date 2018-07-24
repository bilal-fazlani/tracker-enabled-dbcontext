using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Common.Tests;
using TrackerEnabledDbContext.Common.Tests.Extensions;
using TrackerEnabledDbContext.Common.Tests.Code;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class EventTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();

        [TestMethod]
        public void CanRaiseAddEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Added &&
                        args.Log.TypeFullName == typeof (TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var entity = new TrackedModelWithMultipleProperties();

                entity.Description = rdg.Get<string>();

                context.TrackedModelWithMultipleProperties.Add(entity);

                context.SaveChanges();

                //assert
                Assert.IsTrue(eventRaised);

                //make sure log is saved in database
                entity.AssertAuditForAddition(context, entity.Id, null,
                    x => x.Id,
                    x => x.Description);
            }
        }

        [TestMethod]
        public void CanRaiseModifyEvent()
        {
            //TODO: modify test tracker context and identity test tracker context so that on disposal they revert the changes
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool modifyEventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Modified &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        modifyEventRaised = true;
                    }
                };

                var existingEntity = new TrackedModelWithMultipleProperties();

                string originalValue = existingEntity.Name;
                existingEntity.Name = rdg.Get<string>();

                context.SaveChanges();

                //assert
                Assert.IsTrue(modifyEventRaised);

                existingEntity.AssertAuditForModification(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.Name),
                        OriginalValue = originalValue,
                        NewValue = existingEntity.Name
                    });
            }
        }

        [TestMethod]
        public void CanRaiseDeleteEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<NormalModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as NormalModel;

                    if (args.Log.EventType == EventType.Deleted &&
                        args.Log.TypeFullName == typeof(NormalModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = new NormalModel();

                context.NormalModels.Remove(existingEntity);
                context.SaveChanges();

                //assert
                Assert.IsTrue(eventRaised);

                existingEntity.AssertAuditForDeletion(context, existingEntity.Id, null,
                    x => x.Description,
                    x => x.Id);
            }
        }

        [TestMethod]
        public void CanRaiseSoftDeleteEvent()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (x=>x.IsDeleted);

            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<SoftDeletableModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as SoftDeletableModel;

                    if (args.Log.EventType == EventType.SoftDeleted &&
                        args.Log.TypeFullName == typeof(SoftDeletableModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = new SoftDeletableModel();

                existingEntity.Delete();

                context.SaveChanges();

                //assert
                Assert.IsTrue(eventRaised);

                existingEntity.AssertAuditForSoftDeletion(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.IsDeleted),
                        OriginalValue = false.ToString(),
                        NewValue = true.ToString()
                    });
            }
        }

        [TestMethod]
        public void CanRaiseUnDeleteEvent()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (x => x.IsDeleted);

            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<SoftDeletableModel>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as SoftDeletableModel;

                    if (args.Log.EventType == EventType.UnDeleted &&
                        args.Log.TypeFullName == typeof(SoftDeletableModel).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                    }
                };

                var existingEntity = new SoftDeletableModel();
                
                existingEntity.Delete();

                context.SaveChanges();

                //now undelete
                existingEntity.IsDeleted = false;
                context.SaveChanges();

                //assert
                Assert.IsTrue(eventRaised);

                existingEntity.AssertAuditForUndeletion(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.IsDeleted),
                        OriginalValue = true.ToString(),
                        NewValue = false.ToString()
                    });
            }
        }

        [TestMethod]
        public void CanSkipTrackingUsingEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Added &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventRaised = true;
                        args.SkipSavingLog = true;
                    }
                };

                var entity = new TrackedModelWithMultipleProperties();

                //assert
                Assert.IsTrue(eventRaised);

                //make sure log is saved in database
                entity.AssertNoLogs(context, entity.Id, EventType.Added);
            }
        }

        [TestMethod]
        public void CanChangeEntityInEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                bool eventRaised = false;

                string modifiedValue = rdg.Get<string>();

                context.OnAuditLogGenerated += (sender, args) =>
                {
                    var eventEntity = args.Entity as TrackedModelWithMultipleProperties;

                    if (args.Log.EventType == EventType.Modified &&
                        args.Log.TypeFullName == typeof(TrackedModelWithMultipleProperties).FullName &&
                        eventEntity != null)
                    {
                        eventEntity.Name = modifiedValue;
                        eventRaised = true;
                    }
                };

                TrackedModelWithMultipleProperties existingEntity = new TrackedModelWithMultipleProperties
                {
                    Name = rdg.Get<string>()
                };
                context.TrackedModelWithMultipleProperties.Add(existingEntity);
                context.SaveChanges();

                string originalValue = existingEntity.Name;
                existingEntity.Name = modifiedValue;

                context.TrackedModelWithMultipleProperties.Update(existingEntity);
                context.SaveChanges();
                
                //assert
                Assert.IsTrue(eventRaised);

                existingEntity.AssertAuditForModification(context, existingEntity.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(existingEntity.Name),
                        OriginalValue = originalValue,
                        NewValue = modifiedValue
                    });

                context.Entry(existingEntity).Reload();
                Assert.AreEqual(existingEntity.Name, modifiedValue);
            }
        }

        private TestTrackerContext GetNewContextInstance()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            return new TestTrackerContext(options);
        }
    }
}
