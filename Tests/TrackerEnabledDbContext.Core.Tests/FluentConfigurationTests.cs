using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Tests.Code;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Common.Tests;
using TrackerEnabledDbContext.Core.Common.Tests.Extensions;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class FluentConfigurationTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";
        protected RandomDataGenerator rdg = new RandomDataGenerator();

        [TestMethod]
        public void Can_recognise_global_tracking_indicator_when_disabled()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            GlobalTrackingConfig.Enabled = false;

            EntityTracker
                .TrackAllProperties<POCO>()
                .Except(x=>x.StartTime)
                .And(x=>x.Color);

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                POCO model = new POCO();
                ttc.POCOes.Add(model);
                ttc.SaveChanges();

                model.AssertNoLogs(ttc, model.Id);
            }
        }

        [TestMethod]
        public void Can_recognise_global_tracking_indicator_when_enabled()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            EntityTracker
                .TrackAllProperties<POCO>();

            POCO model = new POCO
            {
                Color = "Red",
                Height = 67.4,
                StartTime = new DateTime(2015, 5, 5)
            };

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.POCOes.Add(model);
                ttc.SaveChanges();

                model.AssertAuditForAddition(ttc, model.Id, null,
                    x => x.Color,
                    x => x.Id,
                    x => x.Height,
                    x => x.StartTime);
            }
        }

        [TestMethod]
        public async Task Can_Override_annotation_based_configuration_for_entity_skipTracking()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var model = new NormalModel();
            EntityTracker
                .OverrideTracking<NormalModel>()
                .Disable();

            string userName = rdg.Get<string>();

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.NormalModels.Add(model);
                await ttc.SaveChangesAsync(userName);

                model.AssertNoLogs(ttc, model.Id);
            }
        }

        [TestMethod]
        public void Can_Override_annotation_based_configuration_for_property()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            var model = new TrackedModelWithMultipleProperties
            {
                Category = rdg.Get<Char>(), //tracked ->skipped
                Description = rdg.Get<string>(), //skipped
                IsSpecial = true, //tracked -> skipped
                StartDate = new DateTime(2015, 5, 5), //skipped
                Name = rdg.Get<string>(), //tracked
                Value = rdg.Get<int>() //skipped -> Tracked
            };

            EntityTracker
                .OverrideTracking<TrackedModelWithMultipleProperties>()
                //enable vaue
                .Enable(x => x.Value)
                //disable for isSpecial
                .Disable(x => x.IsSpecial)
                //disable category
                .Disable(x=>x.Category);

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.TrackedModelWithMultipleProperties.Add(model);

                string userName = rdg.Get<string>();

                ttc.SaveChanges(userName);

                model.AssertAuditForAddition(ttc, model.Id, userName,
                    x => x.Id,
                    x => x.Name,
                    x => x.Value);
            }
        }

        //TODO: can track CHAR properties ? NO
    }
}
