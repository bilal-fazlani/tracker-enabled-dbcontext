using System;
using Microsoft.EntityFrameworkCore;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Tests.Models;
using TrackerEnabledDbContext.Core.Common.Tests;

namespace TrackerEnabledDbContext.Core.Tests
{
    public class TestTrackerContext : TrackerContext, ITestDbContext
    {
        public TestTrackerContext(DbContextOptions<TestTrackerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO: Core complained and these are the temp solutions
            modelBuilder.Entity<ModelWithCompositeKey>()
                .HasKey(m => new { m.Key1, m.Key2 });

            modelBuilder.Entity<TrackedModelWithMultipleProperties>()
                .Ignore(m => m.Category);

            modelBuilder.Entity<ModelWithComplexType>()
                .OwnsOne(m => m.ComplexType);
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