using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Common.Tests;
using TrackerEnabledDbContext.Core.Common.Tests.Extensions;

namespace TrackerEnabledDbContext.Core.Tests
{
    [TestClass]
    public class MetadataTests
    {
        protected static readonly string TestConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TEDB-Tests;Integrated Security=True;MultipleActiveResultSets=true";

        [TestMethod]
        public void ShouldAddSingleMetadata_WhenSingleMetadataIsProvided()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureMetadata(m =>
                {
                    m.IpAddress = "192.168.2.23";
                });

                EntityTracker.TrackAllProperties<POCO>();
                POCO entity = new POCO();

                ttc.POCOes.Add(entity);
                ttc.SaveChanges("bilal");

                entity.AssertAuditForAddition(ttc, entity.Id, "bilal",
                    x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

                entity.AssertMetadata(ttc, entity.Id, new Dictionary<string, string>
                {
                    ["IpAddress"] = "192.168.2.23"
                });
            }
        }

        [TestMethod]
        public async Task ShouldAddSingleMetadata_WhenSingleMetadataIsProvided_Async()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureMetadata(m =>
                {
                    m.IpAddress = "192.168.2.23";
                });

                EntityTracker.TrackAllProperties<POCO>();
                POCO entity = new POCO();

                ttc.POCOes.Add(entity);
                await ttc.SaveChangesAsync("bilal");

                entity.AssertAuditForAddition(ttc, entity.Id, "bilal",
                    x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

                entity.AssertMetadata(ttc, entity.Id, new Dictionary<string, string>
                {
                    ["IpAddress"] = "192.168.2.23"
                });
            }
        }

        [TestMethod]
        public void ShouldNotAddMetadata_WhenValueIsNull()
        {
            var options = new DbContextOptionsBuilder<TestTrackerContext>()
                    .UseSqlServer(TestConnectionString)
                    .Options;

            using (TestTrackerContext ttc = new TestTrackerContext(options))
            {
                ttc.ConfigureMetadata(m =>
                {
                    m.IpAddress = "192.168.2.23";
                    m.Country = null;
                    m.Device = string.Empty;
                });

                EntityTracker.TrackAllProperties<POCO>();
                POCO entity = new POCO();

                ttc.POCOes.Add(entity);
                ttc.SaveChanges();

                entity.AssertAuditForAddition(ttc, entity.Id, null,
                    x => x.Color, x => x.Height, x => x.StartTime, x => x.Id);

                entity.AssertMetadata(ttc, entity.Id, new Dictionary<string, string>
                {
                    ["IpAddress"] = "192.168.2.23",
                    ["Device"] = string.Empty
                });
            }
        }
    }
}