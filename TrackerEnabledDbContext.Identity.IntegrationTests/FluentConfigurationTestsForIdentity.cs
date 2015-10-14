using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    [TestClass]
    public class FluentConfigurationTestsForIdentity : PersistanceTests<TestTrackerIdentityContext>
    {
        [TestMethod]
        public void Can_recognise_global_tracking_indicator_when_disabled()
        {
            GlobalTrackingConfig.Enabled = false;
            EntityTracker
                .TrackAllProperties<POCO>();

            POCO model = ObjectFactory<POCO>.Create();
            db.POCOs.Add(model);
            db.SaveChanges();

            model.AssertNoLogs(db, model.Id);
        }

        [TestMethod]
        public void Can_recognise_global_tracking_indicator_when_enabled()
        {
            EntityTracker.TrackAllProperties<POCO>();

            POCO model = new POCO
            {
                Color = "Red",
                Height = 67.4,
                StartTime = new DateTime(2015, 5, 5)
            };

            db.POCOs.Add(model);
            db.SaveChanges();

            model.AssertAuditForAddition(db, model.Id, null,
                x=>x.Color,
                x=>x.Id,
                x=>x.Height,
                x=>x.StartTime);
        }

        [TestMethod]
        public async Task Can_Override_annotation_based_configuration_for_entity_skipTracking()
        {
            var model = new NormalModel();
            EntityTracker
                .OverrideTracking<NormalModel>()
                .Disable();

            string userName = RandomText;

            db.NormalModels.Add(model);
            await db.SaveChangesAsync(userName);

            model.AssertNoLogs(db, model.Id);
        }

        [TestMethod]
        public void Can_Override_annotation_based_configuration_for_property()
        {
            var model = new TrackedModelWithMultipleProperties
            {
                Category = RandomChar, //tracked ->skipped
                Description = RandomText, //skipped
                IsSpecial = true, //tracked -> skipped
                StartDate = new DateTime(2015, 5, 5), //skipped
                Name = RandomText, //tracked
                Value = RandomNumber //skipped -> Tracked
            };

            EntityTracker
                .OverrideTracking<TrackedModelWithMultipleProperties>()
                //enable tracking for value
                .Enable(x => x.Value)
                //disable for isSpecial
                .Disable(x => x.IsSpecial)
                .Disable(x => x.Category);

            db.TrackedModelsWithMultipleProperties.Add(model);

            string userName = RandomText;

            db.SaveChanges(userName);

            model.AssertAuditForAddition(db, model.Id, userName,
                x => x.Id,
                x => x.Name,
                x => x.Value);
        }

        //TODO: can track CHAR properties ? NO
    }
}
