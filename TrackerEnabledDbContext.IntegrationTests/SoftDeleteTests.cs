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
        [TestMethod]
        public void ShouldCreateSoftDeleteLog()
        {
            //setup soft deletable config
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (entity => entity.IsDeleted);

            //create a softdeletable entity and soft delete it
            var deletable = new SoftDeletableModel();

            db.SoftDeletableModels.Add(deletable);

            //save it to database
            db.SaveChanges();

            deletable.AssertAuditForAddition(db, deletable.Id,
                null,x=>x.Id, x=>x.IsDeleted);
            
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

        //TODO: test for undelete
    }
}
