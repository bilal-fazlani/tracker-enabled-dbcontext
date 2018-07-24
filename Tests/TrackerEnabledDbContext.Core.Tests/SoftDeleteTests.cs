using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Tests;
using TrackerEnabledDbContext.Common.Tests.Code;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Common.Tests.Extensions;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class SoftDeleteTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();

        [TestInitialize]
        public void InitializeSoftDeletionTests()
        {
            //setup soft deletable config
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>
                (entity => entity.IsDeleted);
        }

        [TestMethod]
        public void ShouldCreateSoftDeleteLog()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //create a softdeletable entity and soft delete it
            var deletable = new SoftDeletableModel();

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.SoftDeletableModels.Add(deletable);

                //save it to database
                ttc.SaveChanges();

                deletable.AssertAuditForAddition(ttc, deletable.Id,
                    null, x => x.Id);

                //soft delete entity
                deletable.Delete();

                //save changes
                ttc.SaveChanges();

                //assert for soft deletion
                deletable.AssertAuditForSoftDeletion(ttc, deletable.Id, null, new AuditLogDetail
                {
                    NewValue = true.ToString(),
                    OriginalValue = false.ToString(),
                    PropertyName = nameof(deletable.IsDeleted)
                });
            }
        }

        [TestMethod]
        public void ShouldCreateSoftDeleteLogForMultiplePropertiesChanged()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            //create a softdeletable entity and soft delete it
            var deletable = new SoftDeletableModel();

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.SoftDeletableModels.Add(deletable);

                //save it to database
                ttc.SaveChanges();

                deletable.AssertAuditForAddition(ttc, deletable.Id,
                    null, x => x.Id);

                //soft delete entity
                deletable.Delete();
                deletable.Description = rdg.Get<string>();

                //save changes
                ttc.SaveChanges();

                //assert for soft deletion
                deletable.AssertAuditForSoftDeletion(ttc, deletable.Id, null, new AuditLogDetail
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
        }

        [TestMethod]
        public void ShouldCreateUnDeletedLog()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var deletable = new SoftDeletableModel
            {
                Description = rdg.Get<string>(),
            };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Set<SoftDeletableModel>().Attach(deletable);
                ttc.Entry(deletable).State = EntityState.Added;
                ttc.SaveChanges();

                deletable.AssertAuditForAddition(ttc, deletable.Id, null,
                    x => x.Id, x => x.Description);

                deletable.Delete();
                ttc.SaveChanges();

                deletable.AssertAuditForSoftDeletion(ttc, deletable.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(deletable.IsDeleted),
                        OriginalValue = false.ToString(),
                        NewValue = true.ToString()
                    });

                deletable.IsDeleted = false;
                ttc.SaveChanges();

                deletable.AssertAuditForUndeletion(ttc, deletable.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(deletable.IsDeleted),
                        OriginalValue = true.ToString(),
                        NewValue = false.ToString()
                    });
            }
        }

        [TestMethod]
        public async Task ShouldCreateUnDeletedLogForMultiplePropertiesChanged()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            string oldDescription = rdg.Get<string>();
            string newDescription = rdg.Get<string>();

            var deletable = new SoftDeletableModel
            {
                Description = oldDescription
            };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.Set<SoftDeletableModel>().Attach(deletable);
                ttc.Entry(deletable).State = EntityState.Added;
                await ttc.SaveChangesAsync();

                deletable.AssertAuditForAddition(ttc, deletable.Id, null,
                    x => x.Id, x => x.Description);

                deletable.Delete();
                await ttc.SaveChangesAsync();

                deletable.AssertAuditForSoftDeletion(ttc, deletable.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(deletable.IsDeleted),
                        OriginalValue = false.ToString(),
                        NewValue = true.ToString()
                    });

                deletable.IsDeleted = false;
                deletable.Description = newDescription;
                await ttc.SaveChangesAsync();

                deletable.AssertAuditForUndeletion(ttc, deletable.Id, null,
                    new AuditLogDetail
                    {
                        PropertyName = nameof(deletable.IsDeleted),
                        OriginalValue = true.ToString(),
                        NewValue = false.ToString()
                    },
                    new AuditLogDetail
                    {
                        PropertyName = nameof(deletable.Description),
                        OriginalValue = oldDescription,
                        NewValue = newDescription
                    });
            }
        }
    }
}
