using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SampleLogMaker.App_Start;
using SampleLogMaker.Models;
using TrackerEnabledDbContext.Common.Configuration;

namespace SampleLogMaker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>(x=>x.IsDeleted);
            EntityTracker.TrackAllProperties<ApplicationUser>();
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuditConfig.Configure();
        }
    }
}
