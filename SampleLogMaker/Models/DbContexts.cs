using System.Data.Entity;
using TrackerEnabledDbContext.Identity;

namespace SampleLogMaker.Models
{
    public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<BlogUser> BlogUsers { get; set; }
    }
}