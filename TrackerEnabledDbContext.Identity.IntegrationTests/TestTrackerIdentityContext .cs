using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Identity.IntegrationTests
{
    public class TestTrackerIdentityContext : TrackerIdentityContext<IdentityUser>, ITestDbContext
    {
        public TestTrackerIdentityContext()
            : base("DefaultTestConnection")
        {
        }

        public DbSet<NormalModel> NormalModels { get; set; }
        public DbSet<ParentModel> ParentModels { get; set; }
        public DbSet<ChildModel> Children { get; set; }
        public DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        public DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
        public DbSet<ModelWithSkipTracking> ModelsWithSkipTracking { get; set; }
    }
}