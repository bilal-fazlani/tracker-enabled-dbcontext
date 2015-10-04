using SampleLogMaker.Models;
using TrackerEnabledDbContext.Common.Configuration;

namespace SampleLogMaker.App_Start
{
    internal static class AuditConfig
    {
        internal static void Configure()
        {
            EntityTrackingConfiguration<Comment>
                .PauseTracking();
        }
    }
}
