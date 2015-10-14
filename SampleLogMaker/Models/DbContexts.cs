using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using TrackerEnabledDbContext.Common.Configuration;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //using the Fluent Configuration API
            modelBuilder.Entity<Comment>().TrackAllProperties().Except(x => x.Text);
            // alternately could call
            // modelBuilder.Configurations.Add(new CommentConfiguration());
        }
    }

    // Sample EntityTypeConfiguration Class
    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration()
        {
            this.TrackAllProperties().Except(p => p.Text);
        }
    }
}