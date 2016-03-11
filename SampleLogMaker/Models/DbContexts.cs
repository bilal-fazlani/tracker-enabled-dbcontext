using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.IO;
using Serilog;
using TrackerEnabledDbContext.Common.Extensions;
using TrackerEnabledDbContext.Identity;

namespace SampleLogMaker.Models
{
    public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log-{Date}.txt");

            ILogger logger = new LoggerConfiguration()
                .WriteTo
                .RollingFile(filePath) 
                //here you can add
                //elastic search
                //mongodb
                //seq,
                //or any serilog sink you want
                .CreateLogger();

            AddLogger(logger, "@{log}", loginfo=> new object[] {loginfo});

            //stop saving audit logs to applicaiton database
            OnAuditLogGenerated += (sender, args) =>
            {
                args.SkipSaving = true;
                args.SkipSavingLogToSerilog = false; // <-- this is false by default
            };
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