using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.EF6.Common.Tests;

namespace TrackerEnabledDbContext.EF6.Identity.Tests
{
    public class TestTrackerIdentityContext : TrackerIdentityContext<IdentityUser>, ITestDbContext
    {
        protected static readonly string TestConnectionString = Environment.GetEnvironmentVariable("TestGenericConnectionString")
            ?? "DefaultTestConnection";

        public TestTrackerIdentityContext()
            : base(TestConnectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<TestTrackerIdentityContext>());
        }

        public DbSet<NormalModel> NormalModels { get; set; }
        public DbSet<ParentModel> ParentModels { get; set; }
        public DbSet<ChildModel> ChildModels { get; set; }
        public DbSet<ModelWithCompositeKey> ModelWithCompositeKeys { get; set; }
        public DbSet<ModelWithConventionalKey> ModelWithConventionalKeys { get; set; }
        public DbSet<ModelWithSkipTracking> ModelWithSkipTrackings { get; set; }
        public DbSet<POCO> POCOes { get; set; }
        public DbSet<TrackedModelWithMultipleProperties> TrackedModelWithMultipleProperties { get; set; }
        public DbSet<TrackedModelWithCustomTableAndColumnNames> TrackedModelWithCustomTableAndColumnNames { get; set; }
        public DbSet<SoftDeletableModel> SoftDeletableModels { get; set; }

        public DbSet<ModelWithComplexType> ModelWithComplexTypes { get; set; }
    }
}