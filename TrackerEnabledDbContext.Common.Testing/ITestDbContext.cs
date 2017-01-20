using System.Data.Entity;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Common.Testing
{
    public interface ITestDbContext : ITrackerContext
    {
        DbSet<NormalModel> NormalModels { get; set; }
        DbSet<ParentModel> ParentModels { get; set; }
        DbSet<ChildModel> Children { get; set; }
        DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
        DbSet<ModelWithSkipTracking> ModelsWithSkipTracking { get; set; }
        DbSet<POCO> POCOs { get; set; }
        DbSet<TrackedModelWithMultipleProperties> TrackedModelsWithMultipleProperties { get; set; }
        DbSet<TrackedModelWithCustomTableAndColumnNames> TrackedModelsWithCustomTableAndColumnNames { get; set; }
        DbSet<SoftDeletableModel> SoftDeletableModels { get; set; }
        DbSet<ModelWithComplexType> ModelsWithComplexType { get; set; }
    }
}