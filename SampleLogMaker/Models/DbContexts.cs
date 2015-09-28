using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Fluent;
using TrackerEnabledDbContext.Identity;

namespace SampleLogMaker.Models
{
    public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        //static ApplicationDbContext()
        //{
        //    //TrackerConfiguration<Comment>
        //    //    .EnableTableTracking()
        //    //    .SkipTrackingForColumn(x => x.Id)
        //    //    .SkipTrackingForColumn(x => x.ParentBlogId);
        //}

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}