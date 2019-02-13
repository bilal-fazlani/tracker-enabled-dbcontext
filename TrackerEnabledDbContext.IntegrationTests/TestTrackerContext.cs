using System;
using System.Data.Entity;
using TrackerEnabledDbContext.Common.Testing;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.IntegrationTests
{
    public class TestTrackerContext : TrackerContext, ITestDbContext
    {
        protected static readonly string TestConnectionString = Environment.GetEnvironmentVariable("TestGenericConnectionString")
            ?? "DefaultTestConnection";

        public TestTrackerContext()
            : base(TestConnectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<TestTrackerContext>());
        }

        public DbSet<NormalModel> NormalModels { get; set; }
        public DbSet<ParentModel> ParentModels { get; set; }
        public DbSet<ChildModel> Children { get; set; }
        public DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        public DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
        public DbSet<ModelWithSkipTracking> ModelsWithSkipTracking { get; set; }
        public DbSet<POCO> POCOs { get; set; }
        public DbSet<TrackedModelWithMultipleProperties> TrackedModelsWithMultipleProperties { get; set; }
        public DbSet<TrackedModelWithCustomTableAndColumnNames> TrackedModelsWithCustomTableAndColumnNames { get; set; }
        public DbSet<SoftDeletableModel> SoftDeletableModels { get; set; }
        public DbSet<ModelWithComplexType> ModelsWithComplexType { get; set; }
        public DbSet<ExtendedModel> ExtendedModels { get; set; }
        public DbSet<TrackedExtendedModel> TrackedExtendedModels { get; set; }
    }
}