using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackerEnabledDbContext.Common.Testing;

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

            
        }
    }
}