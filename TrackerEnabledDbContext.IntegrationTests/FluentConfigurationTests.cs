﻿using System;
using System.Collections.Generic;
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
    public class FluentConfigurationTests : PersistanceTests<TestTrackerContext>
    {
        [TestMethod]
        public void Can_recognise_global_addition_tracking_indicator_when_disabled()
        {
            GlobalTrackingConfig.AdditionsEnabled = false;

            EntityTracker
                .TrackAllProperties<POCO>()
                .Except(x=>x.StartTime)
                .And(x=>x.Color);

            POCO model = ObjectFactory.Create<POCO>();
            Db.POCOs.Add(model);
            Db.SaveChanges();

            model.AssertNoLogs(Db, model.Id);
        }

        [TestMethod]
        public void Can_recognise_global_addition_tracking_indicator_when_enabled()
        {
            EntityTracker
                .TrackAllProperties<POCO>();

            POCO model = new POCO
            {
                Color = "Red",
                Height = 67.4,
                StartTime = new DateTime(2015, 5, 5)
            };

            Db.POCOs.Add(model);
            Db.SaveChanges();

            model.AssertAuditForAddition(Db, model.Id, null,
                x=>x.Color,
                x=>x.Id,
                x=>x.Height,
                x=>x.StartTime);
        }

        [TestMethod]
        public void Can_recognise_global_change_tracking_indicator_when_disabled()
        {
            GlobalTrackingConfig.ModificationsEnabled = false;

            EntityTracker
                .TrackAllProperties<POCO>()
                .Except(x => x.StartTime)
                .And(x => x.Color);

            POCO model = ObjectFactory.Create<POCO>();
            Db.POCOs.Add(model);
            Db.SaveChanges();
            model.Height++;
            Db.SaveChanges();

            model.AssertNoLogs(Db, model.Id, Common.Models.EventType.Modified);
        }

        [TestMethod]
        public void Can_recognise_global_change_tracking_indicator_when_enabled()
        {
            EntityTracker
                .TrackAllProperties<POCO>();

            POCO model = new POCO
            {
                Color = "Red",
                Height = 67.4,
                StartTime = new DateTime(2015, 5, 5)
            };

            Db.POCOs.Add(model);
            Db.SaveChanges();
            model.Color = "Green";
            Db.SaveChanges();

            model.AssertAuditForModification(Db, model.Id, null, new AuditLogDetail
            {
                NewValue = "Green",
                OriginalValue = "Red",
                PropertyName = "Color"
            });
        }

        [TestMethod]
        public void Can_recognise_global_deletion_tracking_indicator_when_disabled()
        {
            GlobalTrackingConfig.DeletionsEnabled = false;

            EntityTracker
                .TrackAllProperties<POCO>()
                .Except(x => x.StartTime)
                .And(x => x.Color);

            POCO model = ObjectFactory.Create<POCO>();
            Db.POCOs.Add(model);
            Db.SaveChanges();
            Db.POCOs.Remove(model);
            Db.SaveChanges();

            model.AssertNoLogs(Db, model.Id, Common.Models.EventType.Modified);
        }

        [TestMethod]
        public void Can_recognise_global_deletion_tracking_indicator_when_enabled()
        {
            EntityTracker
                .TrackAllProperties<POCO>();

            POCO model = new POCO
            {
                Color = "Red",
                Height = 67.4,
                StartTime = new DateTime(2015, 5, 5)
            };

            Db.POCOs.Add(model);
            Db.SaveChanges();
            Db.POCOs.Remove(model);
            Db.ChangeTracker.DetectChanges();
            Db.SaveChanges();

            model.AssertAuditForDeletion(Db, model.Id, null,
                x => x.Id,
                x => x.Color, 
                x => x.Height,
                x => x.StartTime);
        }

        [TestMethod]
        public async Task Can_Override_annotation_based_configuration_for_entity_skipTracking()
        {
            var model = new NormalModel();
            EntityTracker
                .OverrideTracking<NormalModel>()
                .Disable();

            string userName = RandomText;

            Db.NormalModels.Add(model);
            await Db.SaveChangesAsync(userName);

            model.AssertNoLogs(Db,model.Id);
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
                //enable vaue
                .Enable(x => x.Value)
                //disable for isSpecial
                .Disable(x => x.IsSpecial)
                //disable category
                .Disable(x=>x.Category);

            Db.TrackedModelsWithMultipleProperties.Add(model);

            string userName = RandomText;

            Db.SaveChanges(userName);

            model.AssertAuditForAddition(Db, model.Id, userName, 
                x=>x.Id, 
                x=>x.Name, 
                x=>x.Value);
        }

        //TODO: can track CHAR properties ? NO
    }
}
