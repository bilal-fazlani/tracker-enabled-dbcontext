using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class SoftDeleteTests : PersistanceTests<TestTrackerContext>
    {
        [TestInitialize]
        public void InitializeSoftDeletionTests()
        {
            //setup soft deletable config
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (entity => entity.IsDeleted);

            RollBack = false;
        }

        [TestMethod]
        public void ShouldCreateSoftDeleteLog()
        {
            //create a softdeletable entity and soft delete it
            var deletable = new SoftDeletableModel();

            db.SoftDeletableModels.Add(deletable);

            //save it to database
            db.SaveChanges();

            deletable.AssertAuditForAddition(db, deletable.Id,
                null, x=>x.Id);
            
            //soft delete entity
            deletable.Delete();

            //save changes
            db.SaveChanges();

            //assert for soft deletion
            deletable.AssertAuditForSoftDeletion(db, deletable.Id, null, new AuditLogDetail
            {
                NewValue = true.ToString(),
                OriginalValue = false.ToString(),
                PropertyName = nameof(deletable.IsDeleted)
            });
        }

        [TestMethod]
        public void ShouldCreateSoftDeleteLogForMultiplePropertiesChanged()
        { 
            //create a softdeletable entity and soft delete it
            var deletable = new SoftDeletableModel();

            db.SoftDeletableModels.Add(deletable);

            //save it to database
            db.SaveChanges();

            deletable.AssertAuditForAddition(db, deletable.Id,
                null, x => x.Id);

            //soft delete entity
            deletable.Delete();
            deletable.Description = RandomText;

            //save changes
            db.SaveChanges();

            //assert for soft deletion
            deletable.AssertAuditForSoftDeletion(db, deletable.Id, null, new AuditLogDetail
            {
                NewValue = true.ToString(),
                OriginalValue = false.ToString(),
                PropertyName = nameof(deletable.IsDeleted)
            },
            new AuditLogDetail
            {
                NewValue = deletable.Description,
                OriginalValue = null,
                PropertyName = nameof(deletable.Description)
            });
        }

        [TestMethod]
        public void ShouldCreateUnDeletedLog()
        {
            
        }

        //TODO: test for undelete
    }
}
