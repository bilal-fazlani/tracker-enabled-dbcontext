using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Common.Interfaces;
using TrackerEnabledDbContext.Common.Testing.Models;

namespace TrackerEnabledDbContext.Common.Testing
{
    public interface ITestDbContext:ITrackerContext
    {
        DbSet<NormalModel> NormalModels { get; set; }
        DbSet<ParentModel> ParentModels { get; set; }
        DbSet<ChildModel> Children { get; set; }
        DbSet<ModelWithCompositeKey> ModelsWithCompositeKey { get; set; }
        DbSet<ModelWithConventionalKey> ModelsWithConventionalKey { get; set; }
        DbSet<ModelWithSkipTracking> ModelsWithSkipTracking { get; set; }
    }
}
