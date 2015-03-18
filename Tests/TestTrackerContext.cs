using System.Data.Entity;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    public class TestTrackerContext : TrackerEnabledDbContext.TrackerContext
    {
        public TestTrackerContext()
            : base("DefaultTestConnection")
        {
        }

        public DbSet<NormalModel> NormalModels { get; set; }
        public DbSet<ParentModel> ParentModels { get; set; }
        public DbSet<ChildModel> Children { get; set; }
        public DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        public DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
    }
}
