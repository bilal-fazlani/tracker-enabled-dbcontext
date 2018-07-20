using System.Data.Entity;
using TrackerEnabledDbContext.EF6.Common.Interfaces;
using TrackerEnabledDbContext.Common.Tests.Models;

namespace TrackerEnabledDbContext.EF6.Common.Tests
{
    public interface ITestDbContext : ITrackerContext
    {
        DbSet<NormalModel> NormalModels { get; set; }
        DbSet<ParentModel> ParentModels { get; set; }
        DbSet<ChildModel> ChildModels { get; set; }
        DbSet<ModelWithCompositeKey> ModelWithCompositeKeys { get; set; }
        DbSet<ModelWithConventionalKey> ModelWithConventionalKeys { get; set; }
        DbSet<ModelWithSkipTracking> ModelWithSkipTrackings { get; set; }
        DbSet<POCO> POCOes { get; set; }
        DbSet<TrackedModelWithMultipleProperties> TrackedModelWithMultipleProperties { get; set; }
        DbSet<TrackedModelWithCustomTableAndColumnNames> TrackedModelWithCustomTableAndColumnNames { get; set; }
        DbSet<SoftDeletableModel> SoftDeletableModels { get; set; }
        DbSet<ModelWithComplexType> ModelWithComplexTypes { get; set; }
    }
}