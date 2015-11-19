using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class EventTests : PersistanceTests<TestTrackerContext>
    {
        [TestMethod]
        public void CanRaiseAddEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                List<AuditLog> logsCollected = new List<AuditLog>();

                context.AuditLogGenerated += (sender, args) =>
                {
                    logsCollected.Add(args.Log);
                };

                var entity = ObjectFactory<TrackedModelWithMultipleProperties>.Create(false);

                entity.Description = RandomText;
                entity.Name = RandomText;

                context.TrackedModelsWithMultipleProperties.Add(entity);

                context.SaveChanges();

                //assert
                logsCollected.AssertCount(1);

                logsCollected.AssertAny(log =>
                {
                    bool result =
                        log.TypeFullName == entity.GetType().FullName &&
                        log.EventType == EventType.Added &&
                        log.RecordId == entity.Id.ToString() &&
                        log.UserName == null;


                    log.LogDetails.AssertCount(3);

                    log.LogDetails.AssertContainsLogDetail(new AuditLogDetail
                    {
                        PropertyName = nameof(entity.Description),
                        NewValue = entity.Description
                    });

                    log.LogDetails.AssertContainsLogDetail(new AuditLogDetail
                    {
                        PropertyName = nameof(entity.Name),
                        NewValue = entity.Name
                    });

                    log.LogDetails.AssertContainsLogDetail(new AuditLogDetail
                    {
                        PropertyName = nameof(entity.Id),
                        NewValue = entity.Id.ToString()
                    });

                    return result;
                });
            }
        }

        [TestMethod]
        public void CanRaiseModifyEvent()
        {
            using (var context = GetNewContextInstance())
            {
                EntityTracker.TrackAllProperties<TrackedModelWithMultipleProperties>();

                List<AuditLog> logsCollected = new List<AuditLog>();

                context.AuditLogGenerated += (sender, args) =>
                {
                    logsCollected.Add(args.Log);
                };

                var entity = ObjectFactory<TrackedModelWithMultipleProperties>.Create(false);

                entity.Description = RandomText;
                entity.Name = RandomText;

                string originalValue = entity.Name;

                context.TrackedModelsWithMultipleProperties.Add(entity);

                context.SaveChanges();

                entity.Name = RandomText;

                context.SaveChanges();

                //assert
                logsCollected.AssertCount(2); // 1 for add and 2nd for modify

                var modificationLog = logsCollected[1];

                Assert.IsTrue(modificationLog.TypeFullName == entity.GetType().FullName &&
                        modificationLog.EventType == EventType.Modified &&
                        modificationLog.RecordId == entity.Id.ToString() &&
                        modificationLog.UserName == null);

                modificationLog.LogDetails.AssertCount(1);

                modificationLog.LogDetails.AssertContainsLogDetail(new AuditLogDetail
                {
                    PropertyName = nameof(entity.Name),
                    NewValue = entity.Name,
                    OriginalValue = originalValue
                });
            }
        }

        [TestMethod]
        [Ignore]
        public void CanRaiseDeleteEvent()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Ignore]
        public void CanRaiseSoftDeleteEvent()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Ignore]
        public void CanRaiseUnDeleteEvent()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Ignore]
        public void CanSkipTrackingUsingEvent()
        {
            throw new NotImplementedException();
        } 

        private TestTrackerContext GetNewContextInstance()
        {
            return new TestTrackerContext();
        }
    }
}
