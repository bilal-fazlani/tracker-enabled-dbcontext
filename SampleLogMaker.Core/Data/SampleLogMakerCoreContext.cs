using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Core.Identity;

namespace SampleLogMaker.Core.Models
{
    public class SampleLogMakerCoreContext : TrackerIdentityContext<IdentityUser>
    {
        public SampleLogMakerCoreContext (DbContextOptions<SampleLogMakerCoreContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Blog>()
                .HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false)
                .Property<bool>("IsDeleted").HasDefaultValue(false);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess);
        }
        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                var e = entry;
                switch (e.State)
                {
                    case EntityState.Deleted:
                        e.State = EntityState.Modified;
                        e.CurrentValues["IsDeleted"] = true;
                        break;
                }
            }
        }

        public DbSet<Blog> Blog { get; set; }
    }
}
