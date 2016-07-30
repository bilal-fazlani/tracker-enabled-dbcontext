using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common;
using TrackerEnabledDbContext.Common.Models;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class TimeMachineTests: PersistanceTests<TestTrackerContext>
    {
        [TestMethod]
        public async Task Can_get_Snapshot_for_primitives()
        {
            //add model
            NormalModel model = ObjectFactory.Create<NormalModel>(save: true, testDbContext:Db);
            string snapshot1ExpectedDescription = model.Description;

            //take snapshot1
            await Task.Delay(2.Seconds());
            DateTime point1 = DateTime.UtcNow;

            //edit model
            model.Description = "description 2";
            Db.SaveChanges();

            //take snapshot 2
            await Task.Delay(2.Seconds());
            DateTime point2 = DateTime.UtcNow;

            //delete model
            Db.NormalModels.Remove(model);
            Db.SaveChanges();

            //take snapshot 3
            await Task.Delay(2.Seconds());
            DateTime point3 = DateTime.UtcNow;

            TimeMachine tm = new TimeMachine(Db);
            Snapshot<NormalModel> snapshot1 = tm.TimeTravel<NormalModel>(point1, model.Id);
            Snapshot<NormalModel> snapshot2 = tm.TimeTravel<NormalModel>(point2, model.Id);
            Snapshot<NormalModel> snapshot3 = tm.TimeTravel<NormalModel>(point3, model.Id);

            snapshot1.Entity.Description.Should().Be(snapshot1ExpectedDescription);
            snapshot1.LastAuditLog.EventType.Should().Be(EventType.Added);

            snapshot2.Entity.Description.Should().Be("description 2");
            snapshot2.LastAuditLog.EventType.Should().Be(EventType.Modified);

            snapshot3.LastAuditLog.EventType.Should().Be(EventType.Deleted);
            snapshot3.Entity.Description.Should().Be("description 2");
        }
    }
}