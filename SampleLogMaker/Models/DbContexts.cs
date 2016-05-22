using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using TrackerEnabledDbContext.Common.Extensions;
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
            var commentEntity = modelBuilder.Entity<Comment>();
            commentEntity.TrackAllProperties().Except(x => x.Id);
            commentEntity.HasKey(x => x.Id);
            commentEntity.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            commentEntity.Property(x => x.Text).IsRequired();
            commentEntity.HasRequired(x => x.ParentBlog).WithMany(x=>x.Comments).HasForeignKey(x => x.ParentBlogId);

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
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(x => x.Text).IsRequired();
            this.HasRequired(x => x.ParentBlog).WithMany(x => x.Comments).HasForeignKey(x => x.ParentBlogId);
        }
    }
}