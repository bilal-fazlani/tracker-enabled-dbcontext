using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Extensions;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    [TestClass]
    public class MetadataTests : PersistanceTests<TestTrackerContext>
    {
        [TestMethod]
        public void ShouldAddSingleMetadata_WhenSingleMetadataIsProvided()
        {
            Db.ConfigureMetadata(m =>
            {
                m.IpAddress = "192.168.2.23";
            });

            EntityTracker.TrackAllProperties<POCO>();
            POCO entity = ObjectFactory.Create<POCO>();

            Db.POCOs.Add(entity);
            Db.SaveChanges("bilal");

            entity.AssertAuditForAddition(Db, entity.Id, "bilal",
                x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

            entity.AssertMetadata(Db, entity.Id, new Dictionary<string, string>
            {
                ["IpAddress"] = "192.168.2.23"
            });
        }

        [TestMethod]
        public async Task ShouldAddSingleMetadata_WhenSingleMetadataIsProvided_Async()
        {
            Db.ConfigureMetadata(m =>
            {
                m.IpAddress = "192.168.2.23";
            });

            EntityTracker.TrackAllProperties<POCO>();
            POCO entity = ObjectFactory.Create<POCO>();

            Db.POCOs.Add(entity);
            await Db.SaveChangesAsync("bilal");

            entity.AssertAuditForAddition(Db, entity.Id, "bilal",
                x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

            entity.AssertMetadata(Db, entity.Id, new Dictionary<string, string>
            {
                ["IpAddress"] = "192.168.2.23"
            });
        }

        [TestMethod]
        public void ShouldNotAddMetadata_WhenValueIsNull()
        {
            Db.ConfigureMetadata(m =>
            {
                m.IpAddress = "192.168.2.23";
                m.Country = null;
                m.Device = string.Empty;
            });

            EntityTracker.TrackAllProperties<POCO>();
            POCO entity = ObjectFactory.Create<POCO>();

            Db.POCOs.Add(entity);
            Db.SaveChanges();

            entity.AssertAuditForAddition(Db, entity.Id, null,
                x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

            entity.AssertMetadata(Db, entity.Id, new Dictionary<string, string>
            {
                ["IpAddress"] = "192.168.2.23",
                ["Device"] = string.Empty
            });
        }
    }
}